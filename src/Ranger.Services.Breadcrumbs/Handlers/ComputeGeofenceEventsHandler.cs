using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Ranger.Common;
using Ranger.RabbitMQ;
using Ranger.RabbitMQ.BusPublisher;
using Ranger.Services.Breadcrumbs.Data;

namespace Ranger.Services.Breadcrumbs.Handlers
{
    public class ComputeGeofenceEventsHandler : ICommandHandler<ComputeGeofenceEvents>
    {
        private readonly IBusPublisher busPublisher;
        private readonly Func<string, BreadcrumbsRepository> breadcrumbsRepoFactory;
        private readonly ILogger<ComputeGeofenceEventsHandler> logger;

        public ComputeGeofenceEventsHandler(IBusPublisher busPublisher, Func<string, BreadcrumbsRepository> breadcrumbsRepoFactory, ILogger<ComputeGeofenceEventsHandler> logger)
        {
            this.busPublisher = busPublisher;
            this.breadcrumbsRepoFactory = breadcrumbsRepoFactory;
            this.logger = logger;
        }

        public async Task HandleAsync(ComputeGeofenceEvents message, ICorrelationContext context)
        {
            var breadcrumbsRepo = breadcrumbsRepoFactory(message.TenantId);

            var breadcrumbGeofenceResults = await ComputeGeofenceEventResults(message.Breadcrumb, message.ProjectId, message.GeofenceIntersectionIds);

            await breadcrumbsRepo.AddBreadcrumb(new Data.Breadcrumb
            {
                TenantId = message.TenantId,
                ProjectId = message.ProjectId,
                Environment = message.Environment,
                GeofenceResults = breadcrumbGeofenceResults,
                DeviceId = message.Breadcrumb.DeviceId,
                ExternalUserId = message.Breadcrumb.ExternalUserId,
                Position = message.Breadcrumb.Position,
                Accuracy = message.Breadcrumb.Accuracy,
                RecordedAt = message.Breadcrumb.RecordedAt,
                AcceptedAt = message.Breadcrumb.AcceptedAt
            });

            busPublisher.Send(new ComputeGeofenceIntegrations(message.TenantId, message.ProjectId, message.ProjectName, message.Environment, message.Breadcrumb, breadcrumbGeofenceResults), context);

            async Task<IEnumerable<BreadcrumbGeofenceResult>> ComputeGeofenceEventResults(Common.Breadcrumb breadcrumb, Guid projectId, IEnumerable<Guid> geofenceIntersectionIds)
            {
                // get all the breadcrumbs that the user or device did not exit
                var userOrDeviceCurrentlyEnteredBreadcrumbEvents = await breadcrumbsRepo.GetDeviceCurrentlyEnteredBreadcrumbs(breadcrumb, projectId, geofenceIntersectionIds);
                logger.LogDebug("Determined the following geofences were previously entered: {ExitedGeofences}", userOrDeviceCurrentlyEnteredBreadcrumbEvents.Select(_ => _.Item1.GeofenceId));

                // If the event was not in any geofences
                if (!geofenceIntersectionIds.Any())
                {
                    logger.LogDebug("The event occurred outside all defined geofences");
                    var results = new List<BreadcrumbGeofenceResult>();
                    // if all geofences were exited
                    if (!userOrDeviceCurrentlyEnteredBreadcrumbEvents.Any())
                    {
                        logger.LogDebug("No geofences need to be exited");
                        // no geofences need exited
                        results.Add(new BreadcrumbGeofenceResult
                        {
                            GeofenceEvent = GeofenceEventEnum.NONE,
                        });
                        return results;
                    }
                    // there are lingering geofences that need exited
                    else
                    {
                        logger.LogDebug("Exiting all previously entered geofences");
                        var unexitedEnteredBreadcrumbIdsToRemove = new List<int>();
                        foreach (var exited in userOrDeviceCurrentlyEnteredBreadcrumbEvents)
                        {
                            results.Add(new BreadcrumbGeofenceResult
                            {
                                GeofenceId = exited.Item1.GeofenceId,
                                GeofenceEvent = GeofenceEventEnum.EXITED,
                                EnteredBreadcrumbId = exited.Item1.EnteredBreadcrumbId
                            });
                        }

                        //remove all unexitedEnteredBreadcrumbIds
                        var notExitedBreadcrumbsStatesToRemove = userOrDeviceCurrentlyEnteredBreadcrumbEvents.Select(_ => _.Item2).Distinct();
                        logger.LogDebug("Removing the following dangling un-exited, entered breadcrumbs: {UnExitedEnteredBreadcrumbs}", notExitedBreadcrumbsStatesToRemove);
                        await breadcrumbsRepo.RemoveUnexitedEnteredBreadcrumbIds(notExitedBreadcrumbsStatesToRemove);
                        return results;
                    }
                }
                // event occurred in 1 or more geofences
                else
                {
                    logger.LogDebug("The event occurred within defined geofences");
                    var enteredGeofenceResults = userOrDeviceCurrentlyEnteredBreadcrumbEvents;
                    var enteredGeofenceIds = enteredGeofenceResults.Select(_ => _.Item1.GeofenceId);
                    logger.LogDebug("The following geofences were previously ENTERED: {EnteredGeofences}", enteredGeofenceIds);

                    var dwellingGeofenceIds = enteredGeofenceIds.Intersect(geofenceIntersectionIds);
                    var dwellingGeofenceResults = enteredGeofenceResults.Where(r => dwellingGeofenceIds.Contains(r.Item1.GeofenceId));
                    logger.LogDebug("The following geofences are now DWELLING: {EnteredGeofences}", dwellingGeofenceIds);

                    var exitedGeofenceIds = enteredGeofenceIds.Except(geofenceIntersectionIds);
                    var exitedGeofenceResults = enteredGeofenceResults.Where(r => exitedGeofenceIds.Contains(r.Item1.GeofenceId));
                    logger.LogDebug("The following geofences are now EXITED: {ExitedGeofences}", exitedGeofenceIds);

                    var newlyEnteredGeofenceIds = geofenceIntersectionIds.Except(enteredGeofenceIds).Except(dwellingGeofenceIds).Except(exitedGeofenceIds);
                    logger.LogDebug("The following geofences are now ENTERED: {EnteredGeofences}", newlyEnteredGeofenceIds);

                    var results = new List<BreadcrumbGeofenceResult>();

                    foreach (var newlyEntered in newlyEnteredGeofenceIds)
                    {
                        results.Add(new BreadcrumbGeofenceResult
                        {
                            GeofenceId = newlyEntered,
                            GeofenceEvent = GeofenceEventEnum.ENTERED,
                        });
                    }

                    foreach (var dwelling in dwellingGeofenceResults)
                    {
                        results.Add(new BreadcrumbGeofenceResult
                        {
                            GeofenceId = dwelling.Item1.GeofenceId,
                            GeofenceEvent = GeofenceEventEnum.DWELLING,
                            EnteredBreadcrumbId = dwelling.Item1.EnteredBreadcrumbId
                        });
                    }

                    foreach (var exited in exitedGeofenceResults)
                    {
                        results.Add(new BreadcrumbGeofenceResult
                        {
                            GeofenceId = exited.Item1.GeofenceId,
                            GeofenceEvent = GeofenceEventEnum.EXITED,
                            EnteredBreadcrumbId = exited.Item1.EnteredBreadcrumbId
                        });
                    }

                    //remove unexitedEnteredBreadcrumbIds that no longer contain a dwelling result
                    var breadcrumbsToRemove = exitedGeofenceIds.Except(exitedGeofenceIds.Intersect(dwellingGeofenceIds));
                    var notExitedBreadcrumbsStatesToRemove = enteredGeofenceResults.Where(_ => breadcrumbsToRemove.Contains(_.Item1.GeofenceId)).Select(_ => _.Item2).Distinct();
                    logger.LogDebug("Removing the following dangling un-exited, entered breadcrumbs: {UnExitedEnteredBreadcrumbs}", notExitedBreadcrumbsStatesToRemove);
                    await breadcrumbsRepo.RemoveUnexitedEnteredBreadcrumbIds(notExitedBreadcrumbsStatesToRemove);

                    return results;
                }
            }
        }
    }
}