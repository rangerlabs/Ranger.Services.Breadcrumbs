using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ranger.Services.Breadcrumbs.Data.Migrations
{
    public partial class AddGeofenceIdAndEventToUnexitedTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "geofence_event",
                table: "not_exited_breadcrumb_states",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "geofence_id",
                table: "not_exited_breadcrumb_states",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "geofence_event",
                table: "not_exited_breadcrumb_states");

            migrationBuilder.DropColumn(
                name: "geofence_id",
                table: "not_exited_breadcrumb_states");
        }
    }
}
