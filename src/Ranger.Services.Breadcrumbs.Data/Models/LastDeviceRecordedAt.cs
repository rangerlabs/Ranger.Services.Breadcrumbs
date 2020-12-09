using System;
using System.ComponentModel.DataAnnotations;
using Ranger.Common;

namespace Ranger.Services.Breadcrumbs.Data
{
    public class LastDeviceRecordedAt : IRowLevelSecurityDbSet
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public Guid ProjectId { get; set; }
        [Required]
        public string DeviceId { get; set; }
        [Required]
        public DateTime RecordedAt { get; set; }
        [Required]
        public string TenantId { get; set; }
    }
}