using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Ranger.Common;

namespace Ranger.Services.Breadcrumbs.Data
{
    public class BreadcrumbsRepository : IBreadcrumbsRepository
    {
        private readonly BreadcrumbsDbContext context;
        private readonly ILogger<BreadcrumbsRepository> logger;

        public BreadcrumbsRepository(BreadcrumbsDbContext context, ILogger<BreadcrumbsRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task AddBreadcrumb(Breadcrumb breadcrumb)
        {
            if (breadcrumb is null)
            {
                throw new ArgumentNullException($"{nameof(breadcrumb)} was null");
            }

            var breadcrumbEntity = new BreadcrumbEntity
            {
                TenantId = breadcrumb.TenantId,
                ProjectId = breadcrumb.ProjectId,
                Environment = breadcrumb.Environment,
                DeviceId = breadcrumb.DeviceId,
                ExternalUserId = breadcrumb.ExternalUserId,
                Position = JsonConvert.SerializeObject(breadcrumb.Position),
                Accuracy = breadcrumb.Accuracy,
                RecordedAt = breadcrumb.RecordedAt,
            };

            var geofenceResults = breadcrumb.GeofenceResults.Select(r =>
            {
                var result = new BreadcrumbGeofenceResult
                {
                    TenantId = breadcrumb.TenantId,
                    GeofenceId = r.GeofenceId,
                    GeofenceEvent = r.GeofenceEvent,
                    Breadcrumb = breadcrumbEntity,
                };
                if (r.GeofenceEvent is GeofenceEventEnum.NONE)
                {
                    result.EnteredBreadcrumb = null;
                }
                else if (r.EnteredBreadcrumbId is null)
                {
                    result.EnteredBreadcrumb = breadcrumbEntity;
                }
                else
                {
                    result.EnteredBreadcrumbId = r.EnteredBreadcrumbId;
                }
                return result;
            });


            var unexitedEnteredBreadcrumbId = geofenceResults.Any(r => r.GeofenceEvent is GeofenceEventEnum.ENTERED)
                    ? new NotExitedBreadcrumbState
                    {
                        ProjectId = breadcrumb.ProjectId,
                        DeviceId = breadcrumb.DeviceId,
                        TenantId = breadcrumb.TenantId,
                        Breadcrumb = breadcrumbEntity
                    } : null;

            try
            {
                context.BreadcrumbGeofenceResults.AddRange(geofenceResults);
                if (!(unexitedEnteredBreadcrumbId is null))
                {
                    context.NotExitedBreadcrumbStates.Add(unexitedEnteredBreadcrumbId);
                }
                context.Breadcrumbs.Add(breadcrumbEntity);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to save breadcrumb");
                throw;
            }
        }

        public async Task<IEnumerable<(BreadcrumbGeofenceResult, int)>> GetUserOrDeviceCurrentlyEnteredBreadcrumbs(Ranger.Common.Breadcrumb breadcrumb, Guid projectId, IEnumerable<Guid> geofenceIds)
        {
            if (breadcrumb is null)
            {
                throw new ArgumentNullException($"{nameof(breadcrumb)} was null");
            }

            try
            {
                var correlatedUserEnteredEventUnexitedIdResults = new List<(BreadcrumbGeofenceResult, int)>();
                var correlatedUserEnteredEvents = new List<BreadcrumbGeofenceResult>();
                correlatedUserEnteredEvents = await context.BreadcrumbGeofenceResults.FromSqlInterpolated(
                    $@"select id, breadcrumb_id, entered_breadcrumb_id, geofence_id, geofence_event, tenant_id
                       from 
                            (
                                select r.id, r.breadcrumb_id, r.entered_breadcrumb_id, r.geofence_id, r.geofence_event, r.tenant_id, max(r.breadcrumb_id) OVER (PARTITION BY entered_breadcrumb_id) as last_breadcrumb_id
		                        from breadcrumb_geofence_results r, breadcrumbs b
                        		where b.id in 
                        			(
                                        select breadcrumb_id from not_exited_breadcrumb_states
                        				where project_id = {projectId}
                        				and device_id = {breadcrumb.DeviceId}
                                    )
                        		and r.entered_breadcrumb_id = b.id
                            ) as last_results
                        where breadcrumb_id = last_breadcrumb_id"
                ).ToListAsync();
                var breadcrumbIds = correlatedUserEnteredEvents.Select(_ => _.EnteredBreadcrumbId);
                var unexitedResults = context.NotExitedBreadcrumbStates.AsNoTracking().Where(_ => breadcrumbIds.Contains(_.BreadcrumbId)).ToList();

                foreach (var geofenceEvent in correlatedUserEnteredEvents)
                {
                    correlatedUserEnteredEventUnexitedIdResults.Add((geofenceEvent, unexitedResults.Where(_ => _.BreadcrumbId == geofenceEvent.EnteredBreadcrumbId).Single().Id));
                }

                return correlatedUserEnteredEventUnexitedIdResults;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to query Correlated Entered Events");
                throw;
            }
        }

        public async Task RemoveUnexitedEnteredBreadcrumbIds(IEnumerable<int> unExitedEnteredBreadcrumbIds)
        {
            try
            {
                var toAttach = new List<NotExitedBreadcrumbState>();
                foreach (var unexitedEnteredBreadcrumbId in unExitedEnteredBreadcrumbIds)
                {
                    toAttach.Add(new NotExitedBreadcrumbState { Id = unexitedEnteredBreadcrumbId });
                }
                context.NotExitedBreadcrumbStates.AttachRange(toAttach);
                context.NotExitedBreadcrumbStates.RemoveRange(toAttach);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to remove Unexited Entered Breadcrumbs with Ids '{String.Join(",", unExitedEnteredBreadcrumbIds)}'");
                throw;
            }
        }
    }
}