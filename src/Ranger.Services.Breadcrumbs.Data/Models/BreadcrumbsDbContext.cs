using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Ranger.Common;
using Ranger.RabbitMQ;

namespace Ranger.Services.Breadcrumbs.Data
{
    public class BreadcrumbsDbContext : DbContext, IDataProtectionKeyContext, IOutboxStore
    {

        private readonly IDataProtectionProvider dataProtectionProvider;
        public BreadcrumbsDbContext(DbContextOptions<BreadcrumbsDbContext> options, IDataProtectionProvider dataProtectionProvider = null) : base(options)
        {
            this.dataProtectionProvider = dataProtectionProvider;
        }

        public virtual DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
        public virtual DbSet<BreadcrumbEntity> Breadcrumbs { get; set; }
        public virtual DbSet<BreadcrumbGeofenceResult> BreadcrumbGeofenceResults { get; set; }
        public virtual DbSet<DeviceGeofenceState> DeviceGeofenceStates { get; set; }
        public virtual DbSet<LastDeviceRecordedAt> LastDeviceRecordedAts { get; set; }
        public virtual DbSet<OutboxMessage> OutboxMessages { get; set; }
        public virtual DbSet<RangerRabbitMessage> RangerRabbitMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            EncryptingDbHelper encryptionHelper = null;
            if (dataProtectionProvider != null)
            {
                encryptionHelper = new EncryptingDbHelper(this.dataProtectionProvider);
            }

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                // Remove 'AspNet' prefix and convert table name from PascalCase to snake_case. E.g. AspNetRoleClaims -> role_claims
                entity.SetTableName(entity.GetTableName().Replace("AspNet", "").ToSnakeCase());

                // Convert column names from PascalCase to snake_case.
                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(property.Name.ToSnakeCase());
                }

                // Convert primary key names from PascalCase to snake_case. E.g. PK_users -> pk_users
                foreach (var key in entity.GetKeys())
                {
                    key.SetName(key.GetName().ToSnakeCase());
                }

                // Convert foreign key names from PascalCase to snake_case.
                foreach (var key in entity.GetForeignKeys())
                {
                    key.SetConstraintName(key.GetConstraintName().ToSnakeCase());
                }

                // Convert index names from PascalCase to snake_case.
                foreach (var index in entity.GetIndexes())
                {
                    index.SetName(index.GetName().ToSnakeCase());
                }

                encryptionHelper?.SetEncrytedPropertyAccessMode(entity);
            }

            modelBuilder.Entity<BreadcrumbGeofenceResult>(entity =>
            {
                entity.HasOne(_ => _.Breadcrumb)
                    .WithMany(_ => _.BreadcrumbGeofenceResults)
                    .HasForeignKey(_ => _.BreadcrumbId);
            });

            modelBuilder.Entity<BreadcrumbEntity>().HasIndex(_ => _.ProjectId);
            modelBuilder.Entity<BreadcrumbEntity>().HasIndex(_ => _.Environment);
            modelBuilder.Entity<BreadcrumbGeofenceResult>().HasIndex(_ => _.GeofenceId);
            modelBuilder.Entity<BreadcrumbGeofenceResult>().HasIndex(_ => _.GeofenceEvent);
            modelBuilder.Entity<DeviceGeofenceState>().HasIndex(_ => new { _.ProjectId, _.GeofenceId, _.DeviceId }).IsUnique();
            modelBuilder.Entity<LastDeviceRecordedAt>().HasIndex(_ => new { _.ProjectId, _.DeviceId }).IsUnique();
        }
    }
}