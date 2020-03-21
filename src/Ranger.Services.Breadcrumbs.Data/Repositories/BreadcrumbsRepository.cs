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
                throw new ArgumentNullException($"{nameof(breadcrumb)} was null.");
            }

            var breadcrumbEntity = new BreadcrumbEntity
            {
                DatabaseUsername = breadcrumb.DatabaseUsername,
                ProjectId = breadcrumb.ProjectId,
                Environment = breadcrumb.Environment,
                GeofenceResults = JsonConvert.SerializeObject(breadcrumb.GeofenceResults),
                DeviceId = breadcrumb.DeviceId,
                ExternalUserId = breadcrumb.ExternalUserId,
                Position = JsonConvert.SerializeObject(breadcrumb.Position),
                Accuracy = breadcrumb.Accuracy,
                RecordedAt = breadcrumb.RecordedAt,
                CorrelatedEnteredEventIds = JsonConvert.SerializeObject(breadcrumb.CorrelatedEnteredEventIds)
            };
            try
            {
                context.Breadcrumbs.Add(breadcrumbEntity);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to save breadcrumb.");
                throw;
            }
        }

        public async Task<IEnumerable<Breadcrumb>> GetUserOrDeviceCurrentlyEnteredBreadcrumbs(Ranger.Common.Breadcrumb breadcrumb, Guid projectId, IEnumerable<Guid> geofenceIds)
        {
            if (breadcrumb is null)
            {
                throw new ArgumentNullException($"{nameof(breadcrumb)} was null.");
            }

            try
            {
                IEnumerable<BreadcrumbEntity> correlatedEnteredEvent = new List<BreadcrumbEntity>();
                // if an externl id is present, use it 
                // it is unlikely that a recent previous breadcrumb won't have it
                if (!(breadcrumb.ExternalUserId is null))
                {
                    var asdf = await context.Breadcrumbs.FromSqlInterpolated($@"
                        SELECT FROM breadcrumbs
                        WHERE project_id == {projectId}
                        AND external_user_id == {breadcrumb.ExternalUserId}
                        AND correlated_entered_event_id == null
                        AND (geofence_results ->> geofence_ids ?| {geofenceIds.Select(g => g.ToString())})
                        ORDER BY recorded_at DESC
                    ").ToListAsync();
                }

                // fall back to the device id. it's an anonymous breadcrumb
                // the user may have logged out but the correlation can still be made
                // if (correlatedEnteredEvent is null)
                // {
                //     correlatedEnteredEvent = await context.Breadcrumbs.Where(b =>
                //         b.ProjectId == projectId &&
                //         b.DeviceId == breadcrumb.DeviceId &&
                //         b.GeofenceEvent == GeofenceEventEnum.ENTERED &&
                //         b.CorrelatedEnteredEventId == null)
                //         .OrderByDescending(b => b.RecordedAt)
                //         .FirstOrDefaultAsync();
                // }

                return correlatedEnteredEvent.Select(b => new Breadcrumb
                {
                    DatabaseUsername = b.DatabaseUsername,
                    ProjectId = b.ProjectId,
                    Environment = b.Environment,
                    GeofenceResults = JsonConvert.DeserializeObject<IEnumerable<BreadcrumbGeofenceResult>>(b.GeofenceResults),
                    DeviceId = b.DeviceId,
                    ExternalUserId = b.ExternalUserId,
                    Position = JsonConvert.DeserializeObject<LngLat>(b.Position),
                    Accuracy = b.Accuracy,
                    RecordedAt = b.RecordedAt,
                    CorrelatedEnteredEventIds = JsonConvert.DeserializeObject<IEnumerable<int>>(b.CorrelatedEnteredEventIds)
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to query Correlated Entered Events.");
                throw;
            }
        }
    }
}