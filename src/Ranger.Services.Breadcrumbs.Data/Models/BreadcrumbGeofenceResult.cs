using System;
using System.Collections.Generic;
using Ranger.Common;

namespace Ranger.Services.Breadcrumbs.Data
{
    public class BreadcrumbGeofenceResult
    {
        public Guid GeofenceId { get; set; }
        public IEnumerable<Guid> IntegrationIds { get; set; }
        public GeofenceEventEnum GeofenceEvent { get; set; }
    }
}