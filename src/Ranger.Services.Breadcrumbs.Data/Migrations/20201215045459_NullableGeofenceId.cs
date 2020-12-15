using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ranger.Services.Breadcrumbs.Data.Migrations
{
    public partial class NullableGeofenceId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                         name: "geofence_id",
                         table: "breadcrumb_geofence_results",
                         nullable: true,
                         oldClrType: typeof(Guid),
                         oldType: "uuid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "geofence_id",
                table: "breadcrumb_geofence_results",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);
        }
    }
}
