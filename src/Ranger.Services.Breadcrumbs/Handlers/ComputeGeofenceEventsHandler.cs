using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Ranger.Common;
using Ranger.RabbitMQ;
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
            var breadcrumbsRepo = breadcrumbsRepoFactory(message.Domain);

            var breadcrumbGeofenceResults = await ComputeGeofenceEventResults(message.Breadcrumb, message.ProjectId, message.GeofenceIntersectionIds);

            await breadcrumbsRepo.AddBreadcrumb(new Data.Breadcrumb
            {
                DatabaseUsername = message.DatabaseUsername,
                ProjectId = message.ProjectId,
                Environment = message.Environment,
                GeofenceResults = breadcrumbGeofenceResults,
                DeviceId = message.Breadcrumb.DeviceId,
                ExternalUserId = message.Breadcrumb.ExternalUserId,
                Position = message.Breadcrumb.Position,
                Accuracy = message.Breadcrumb.Accuracy,
                RecordedAt = message.Breadcrumb.RecordedAt,
            });

            busPublisher.Send(new ComputeGeofenceIntegrations(message.DatabaseUsername, message.Domain, message.ProjectId, message.Environment, message.Breadcrumb, breadcrumbGeofenceResults), context);

            async Task<IEnumerable<BreadcrumbGeofenceResult>> ComputeGeofenceEventResults(Common.Breadcrumb breadcrumb, Guid projectId, IEnumerable<Guid> geofenceIntersectionIds)
            {
                // get all the breadcrumbs that the user or device did not exit
                var userOrDeviceCurrentlyEnteredBreadcrumbEvents = await breadcrumbsRepo.GetUserOrDeviceCurrentlyEnteredBreadcrumbs(breadcrumb, projectId, geofenceIntersectionIds);

                // If the event was not in any geofences
                if (geofenceIntersectionIds.Count() is 0)
                {
                    var results = new List<BreadcrumbGeofenceResult>();
                    // if all geofences were exited
                    if (userOrDeviceCurrentlyEnteredBreadcrumbEvents.Count() is 0)
                    {
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
                        var enteredGeofenceIds = userOrDeviceCurrentlyEnteredBreadcrumbEvents;

                        var unexitedEnteredBreadcrumbIdsToRemove = new List<int>();
                        foreach (var exited in enteredGeofenceIds)
                        {
                            results.Add(new BreadcrumbGeofenceResult
                            {
                                GeofenceId = exited.Item1.GeofenceId,
                                GeofenceEvent = GeofenceEventEnum.EXITED,
                                EnteredBreadcrumbId = exited.Item1.EnteredBreadcrumbId
                            });
                        }

                        //remove all unexitedEnteredBreadcrumbIds
                        var breadcrumbsToRemove = enteredGeofenceIds.Select(_ => _.Item2).ToList();
                        await breadcrumbsRepo.RemoveUnexitedEnteredBreadcrumbIds(breadcrumbsToRemove);
                        return results;
                    }
                }
                // event occurred in 1 or more geofences
                else
                {
                    var enteredGeofenceResults = userOrDeviceCurrentlyEnteredBreadcrumbEvents;
                    var enteredGeofenceIds = enteredGeofenceResults.Select(_ => _.Item1.GeofenceId);

                    var dwellingGeofenceIds = enteredGeofenceIds.Intersect(geofenceIntersectionIds);
                    var dwellingGeofenceResults = enteredGeofenceResults.Where(r => dwellingGeofenceIds.Contains(r.Item1.GeofenceId));

                    var exitedGeofenceIds = enteredGeofenceIds.Except(geofenceIntersectionIds);
                    var exitedGeofenceResults = enteredGeofenceResults.Where(r => exitedGeofenceIds.Contains(r.Item1.GeofenceId));

                    var newlyEnteredGeofenceIds = geofenceIntersectionIds.Except(enteredGeofenceIds).Except(dwellingGeofenceIds).Except(exitedGeofenceIds);

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
                    await breadcrumbsRepo.RemoveUnexitedEnteredBreadcrumbIds(enteredGeofenceResults.Where(_ => breadcrumbsToRemove.Contains(_.Item1.GeofenceId)).Select(_ => _.Item2));

                    return results;
                }
            }
        }
    }
}