using System;
using Ranger.Common;

namespace Ranger.Services.Breadcrumbs.Models
{
    public class BreadcrumbGeofenceResult
    {
        public Guid GeofenceId { get; set; }
        public GeofenceEventEnum GeofenceEvent { get; set; }
    }
}