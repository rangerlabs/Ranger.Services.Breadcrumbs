using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ranger.Common;

namespace Ranger.Services.Breadcrumbs.Data
{
    public class BreadcrumbEntity : IRowLevelSecurityDbSet
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public string TenantId { get; set; }
        [Required]
        public Guid ProjectId { get; set; }
        [Required]
        public EnvironmentEnum Environment { get; set; }
        public ICollection<BreadcrumbGeofenceResult> BreadcrumbGeofenceResults { get; set; } = new List<BreadcrumbGeofenceResult>();
        [Required]
        public string DeviceId { get; set; }
        public string ExternalUserId { get; set; }
        [Required]
        [Column(TypeName = "jsonb")]
        public string Position { get; set; }
        [Required]
        public double Accuracy { get; set; }
        [Required]
        public DateTime RecordedAt { get; set; }
        [Required]
        public DateTime AcceptedAt { get; set; }
    }
}