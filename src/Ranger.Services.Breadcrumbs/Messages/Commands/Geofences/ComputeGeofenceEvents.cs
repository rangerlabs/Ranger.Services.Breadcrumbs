using System;
using System.Collections.Generic;
using Ranger.Common;
using Ranger.RabbitMQ;

namespace Ranger.Services.Breadcrumbs
{
    [MessageNamespace("breadcrumbs")]
    public class ComputeGeofenceEvents : ICommand
    {
        public string TenantId { get; }
        public Guid ProjectId { get; }
        public string ProjectName { get; }
        public EnvironmentEnum Environment { get; }
        public Breadcrumb Breadcrumb { get; }
        public IEnumerable<Guid> GeofenceIntersectionIds { get; }

        public ComputeGeofenceEvents(
            string tenantId,
            Guid projectId,
            string projectName,
            EnvironmentEnum environment,
            Breadcrumb breadcrumb,
            IEnumerable<Guid> geofenceIntersectionIds
            )
        {
            if (string.IsNullOrWhiteSpace(tenantId))
            {
                throw new ArgumentException($"{nameof(tenantId)} was null or whitespace");
            }

            if (projectId.Equals(Guid.Empty))
            {
                throw new ArgumentException($"{nameof(projectId)} was an empty Guid");
            }

            this.TenantId = tenantId;
            this.ProjectId = projectId;
            this.ProjectName = projectName;
            this.Environment = environment;
            this.Breadcrumb = breadcrumb ?? throw new ArgumentNullException(nameof(breadcrumb));
            this.GeofenceIntersectionIds = geofenceIntersectionIds;
        }
    }
}