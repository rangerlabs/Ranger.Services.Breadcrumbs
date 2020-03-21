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
    public class GeofenceIntersectionsComputedHandler : IEventHandler<GeofenceIntersectionsComputed>
    {
        private readonly Func<string, BreadcrumbsRepository> breadcrumbsRepoFactory;
        private readonly ILogger<GeofenceIntersectionsComputedHandler> logger;

        public GeofenceIntersectionsComputedHandler(Func<string, BreadcrumbsRepository> breadcrumbsRepoFactory, ILogger<GeofenceIntersectionsComputedHandler> logger)
        {
            this.breadcrumbsRepoFactory = breadcrumbsRepoFactory;
            this.logger = logger;
        }

        public async Task HandleAsync(GeofenceIntersectionsComputed message, ICorrelationContext context)
        {
            var breadcrumbsRepo = breadcrumbsRepoFactory(message.Domain);

            var (breadcrumbGeofenceResults, exitedBreadcrumbIds) = await ComputeGeofenceEventResults(message.Breadcrumb, message.ProjectId, message.GeofenceIntegrationResults);

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
                CorrelatedEnteredEventIds = exitedBreadcrumbIds
            });


            async Task<(List<BreadcrumbGeofenceResult>, List<int>)> ComputeGeofenceEventResults(Common.Breadcrumb breadcrumb, Guid projectId, IEnumerable<GeofenceIntegrationResult> geofenceIntegrationResults)
            {
                var geofenceIntegrationResultIds = geofenceIntegrationResults.Select(g => g.GeofenceId);

                // get all the breadcrumbs that the user or device did not exit
                var userOrDeviceCurrentlyEnteredBreadcrumbs = await breadcrumbsRepo.GetUserOrDeviceCurrentlyEnteredBreadcrumbs(breadcrumb, projectId, geofenceIntegrationResults.Select(g => g.GeofenceId));

                // If the event was not in any geofences
                if (geofenceIntegrationResultIds.Count() is 0)
                {
                    // if all geofences were exited
                    if (userOrDeviceCurrentlyEnteredBreadcrumbs.Count() is 0)
                    {
                        var results = (new List<BreadcrumbGeofenceResult>(), new List<int>());
                        // no geofences need exited
                        results.Item1.Add(new BreadcrumbGeofenceResult
                        {
                            GeofenceEvent = GeofenceEventEnum.NONE
                        });
                        return results;
                    }
                    // there are lingering geofences that need exited
                    else
                    {
                        var enteredGeofenceIds = userOrDeviceCurrentlyEnteredBreadcrumbs.SelectMany(b => b.GeofenceResults.Where(r => r.GeofenceEvent is GeofenceEventEnum.ENTERED)).Select(g => g.GeofenceId);

                        //TODO: GET ALL THE INTEGRATIONS FOR ALL GEOFENCES THAT WILL BE EXITED

                        var results = (new List<BreadcrumbGeofenceResult>(), new List<int>());
                        foreach (var exited in enteredGeofenceIds)
                        {
                            results.Item1.Add(new BreadcrumbGeofenceResult
                            {
                                GeofenceId = exited,
                                GeofenceEvent = GeofenceEventEnum.EXITED
                            });
                            results.Item2.Add(userOrDeviceCurrentlyEnteredBreadcrumbs.Where(b => b.GeofenceResults.Any(r => r.GeofenceEvent is GeofenceEventEnum.ENTERED && r.GeofenceId == exited)).Single().Id);
                        }
                        return results;
                    }
                }
                // event occurred in 1 or more geofences
                else
                {
                    var enteredGeofenceIds = userOrDeviceCurrentlyEnteredBreadcrumbs.SelectMany(b => b.GeofenceResults.Where(r => r.GeofenceEvent is GeofenceEventEnum.ENTERED).Select(e => e.GeofenceId));
                    var dwellingGeofenceIds = enteredGeofenceIds.Intersect(geofenceIntegrationResultIds);
                    var exitedGeofenceIds = enteredGeofenceIds.Except(geofenceIntegrationResultIds);
                    var newlyEnteredGeofenceIds = enteredGeofenceIds.Except(dwellingGeofenceIds).Except(exitedGeofenceIds);

                    var results = (new List<BreadcrumbGeofenceResult>(), new List<int>());
                    foreach (var dwelling in dwellingGeofenceIds)
                    {
                        results.Item1.Add(new BreadcrumbGeofenceResult
                        {
                            GeofenceId = dwelling,
                            GeofenceEvent = GeofenceEventEnum.DWELLING
                        });
                    }

                    foreach (var exited in exitedGeofenceIds)
                    {
                        results.Item1.Add(new BreadcrumbGeofenceResult
                        {
                            GeofenceId = exited,
                            GeofenceEvent = GeofenceEventEnum.EXITED
                        });
                        results.Item2.Add(userOrDeviceCurrentlyEnteredBreadcrumbs.Where(b => b.GeofenceResults.Any(r => r.GeofenceEvent is GeofenceEventEnum.ENTERED && r.GeofenceId == exited)).Single().Id);
                    }

                    foreach (var entered in newlyEnteredGeofenceIds)
                    {
                        results.Item1.Add(new BreadcrumbGeofenceResult
                        {
                            GeofenceId = entered,
                            GeofenceEvent = GeofenceEventEnum.ENTERED
                        });
                    }
                    return results;
                }
            }
        }
    }
}