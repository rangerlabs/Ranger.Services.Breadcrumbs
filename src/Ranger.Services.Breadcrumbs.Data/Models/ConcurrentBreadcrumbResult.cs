using System;
using Ranger.Common;

namespace Ranger.Services.Breadcrumbs.Data
{
    public class ConcurrentBreadcrumbResult
    {
        public ConcurrentBreadcrumbResult(Guid projectId, string deviceId, Guid geofenceId, GeofenceEventEnum lastEvent)
        {
            this.ProjectId = projectId;
            this.GeofenceId = geofenceId;
            this.DeviceId = deviceId;
            this.LastEvent = lastEvent;
        }

        public Guid ProjectId { get; }
        public Guid GeofenceId { get; }
        public string DeviceId { get; }
        public GeofenceEventEnum LastEvent { get; }
    }
}