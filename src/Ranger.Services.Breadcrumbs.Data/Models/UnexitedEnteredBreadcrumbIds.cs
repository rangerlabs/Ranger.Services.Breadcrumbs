using System;
using System.ComponentModel.DataAnnotations;
using Ranger.Common;

namespace Ranger.Services.Breadcrumbs.Data
{
    public class NotExitedBreadcrumbState : IRowLevelSecurityDbSet
    {
        [Key]
        public int Id { get; set; }
        public Guid ProjectId { get; set; }
        public int BreadcrumbId { get; set; }
        public BreadcrumbEntity Breadcrumb { get; set; }
        [Required]
        public string DeviceId { get; set; }
        [Required]
        public string TenantId { get; set; }
    }
}