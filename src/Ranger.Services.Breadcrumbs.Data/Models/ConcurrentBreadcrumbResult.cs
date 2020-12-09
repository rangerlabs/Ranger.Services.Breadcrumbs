using System;
using Ranger.Common;

namespace Ranger.Services.Breadcrumbs.Data
{
    public class ConcurrentBreadcrumbResult
    {
        public ConcurrentBreadcrumbResult(Guid projectId, string deviceId, Guid geofenceId, GeofenceEventEnum lastEvent)
        {
            this.ProjectId = projectId;
            this.DeviceId = deviceId;
            this.GeofenceId = geofenceId;
            this.LastEvent = lastEvent;
        }

        public Guid ProjectId { get; }
        public string DeviceId { get; }
        public Guid GeofenceId { get; }
        public GeofenceEventEnum LastEvent { get; }
    }
}