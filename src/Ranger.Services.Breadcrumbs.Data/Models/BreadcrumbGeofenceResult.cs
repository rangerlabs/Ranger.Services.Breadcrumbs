using System;
using System.ComponentModel.DataAnnotations;
using Ranger.Common;

namespace Ranger.Services.Breadcrumbs.Data
{
    public class BreadcrumbGeofenceResult : IRowLevelSecurityDbSet
    {
        [Key]
        public int Id { get; set; }
        public int BreadcrumbId { get; set; }
        public BreadcrumbEntity Breadcrumb { get; set; }
        public int? EnteredBreadcrumbId { get; set; }
        public BreadcrumbEntity EnteredBreadcrumb { get; set; }
        [Required]
        public Guid GeofenceId { get; set; }
        [Required]
        public GeofenceEventEnum GeofenceEvent { get; set; }
        [Required]
        public string TenantId { get; set; }
    }
}