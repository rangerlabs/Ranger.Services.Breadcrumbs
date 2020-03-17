using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ranger.Common;

namespace Ranger.Services.Breadcrumbs.Data
{
    public class Breadcrumb : IRowLevelSecurityDbSet
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string DatabaseUsername { get; set; }
        [Required]
        public Guid ProjectId { get; set; }
        [Required]
        public EnvironmentEnum Environment { get; set; }
        [Required]
        public Guid GeofenceId { get; set; }
        [Required]
        public GeofenceEventEnum GeofenceEvent { get; set; }
        [Required]
        public string DeviceId { get; set; }
        [Required]
        public string ExternalUserId { get; set; }
        [Required]
        public string Position { get; set; }
        [Required]
        public double Accuracy { get; set; }
        [Required]
        public DateTime RecordedAt { get; set; }
        [Column(TypeName = "jsonb")]
        public string Metadata { get; set; }
        public int? CorrelatedEnteredEventId { get; set; }
    }
}