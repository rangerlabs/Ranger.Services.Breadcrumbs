using System;
using System.ComponentModel.DataAnnotations;
using Ranger.Common;

namespace Ranger.Services.Breadcrumbs.Data
{
    public class BreadcrumbGeofenceResult : IRowLevelSecurityDbSet
    {
        [Key]
        public long Id { get; set; }
        public long BreadcrumbId { get; set; }
        public BreadcrumbEntity Breadcrumb { get; set; }
        public Guid? GeofenceId { get; set; }
        [Required]
        public GeofenceEventEnum GeofenceEvent { get; set; }
        [Required]
        public string TenantId { get; set; }
    }
}