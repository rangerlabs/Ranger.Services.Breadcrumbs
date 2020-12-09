using System;
using System.Collections.Generic;
using Ranger.Common;

namespace Ranger.Services.Breadcrumbs.Data
{
    public class Breadcrumb : IRowLevelSecurityDbSet
    {
        public int Id { get; set; }
        public string TenantId { get; set; }
        public Guid ProjectId { get; set; }
        public EnvironmentEnum Environment { get; set; }
        public string DeviceId { get; set; }
        public string ExternalUserId { get; set; }
        public LngLat Position { get; set; }
        public double Accuracy { get; set; }
        public DateTime RecordedAt { get; set; }
        public DateTime AcceptedAt { get; set; }
    }
}