using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Npgsql;
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

        public async Task<long> AddBreadcrumbAndBreadcrumbGeofenceResults(Data.Breadcrumb breadcrumb, ICollection<BreadcrumbGeofenceResult> results)
        {
            if (breadcrumb is null)
            {
                throw new ArgumentNullException($"{nameof(breadcrumb)} was null");
            }

            if (results is null || !results.Any())
            {
                throw new ArgumentNullException($"{nameof(results)} was null or empty");
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
                AcceptedAt = breadcrumb.AcceptedAt
            };

            var geofenceResults = results.Select(r =>
                new BreadcrumbGeofenceResult
                {
                    TenantId = breadcrumb.TenantId,
                    GeofenceId = r.GeofenceId,
                    GeofenceEvent = r.GeofenceEvent,
                    Breadcrumb = breadcrumbEntity,
                }
            ).ToList();
            breadcrumbEntity.BreadcrumbGeofenceResults = geofenceResults;

            try
            {
                context.BreadcrumbGeofenceResults.AddRange(results);
                context.Breadcrumbs.Add(breadcrumbEntity);
                await context.SaveChangesAsync();
                return breadcrumb.Id;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to save breadcrumb and results, tracking was unnaffected");
                throw;
            }
        }

        public async Task<IList<ConcurrentBreadcrumbResult>> UpsertGeofenceStates(string tenantId, Guid projectId, string deviceId, IEnumerable<Guid> geofenceIds, DateTime recordedAt)
        {
            using var connection = new NpgsqlConnection(context.Database.GetDbConnection().ConnectionString);
            using var cmd = new NpgsqlCommand("SELECT upsert_device_geofence_states(@tenant_id, @project_id, @device_id, @geofence_ids, @recorded_at)", connection);
            cmd.Parameters.AddWithValue("@tenant_id", tenantId);
            cmd.Parameters.AddWithValue("@project_id", projectId);
            cmd.Parameters.AddWithValue("@device_id", deviceId);
            cmd.Parameters.AddWithValue("@geofence_ids", geofenceIds);
            cmd.Parameters.AddWithValue("@recorded_at", recordedAt);

            await connection.OpenAsync();
            try
            {
                using var reader = await cmd.ExecuteReaderAsync();

                var concurrentBreadcrumbResults = new List<ConcurrentBreadcrumbResult>();
                while (reader.Read())
                {
                    int index = 0;
                    concurrentBreadcrumbResults.Add(new ConcurrentBreadcrumbResult(reader.GetGuid(index++), reader.GetGuid(index++), reader.GetString(index++), Enum.Parse<GeofenceEventEnum>(reader.GetInt32(index).ToString())));
                }
                reader.Close();
                return concurrentBreadcrumbResults;
            }
            catch (PostgresException ex)
            {
                if (ex.SqlState == "50001")
                {
                    throw new RangerException("The breadcrumb was outdated", ex);
                }
                throw;
            }
        }
    }
}