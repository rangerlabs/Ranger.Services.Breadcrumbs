using Microsoft.EntityFrameworkCore.Migrations;

namespace Ranger.Services.Breadcrumbs.Data.Migrations
{
    public partial class AddUpsertGeofenceStatesFunction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(MigrationMethods.UpsertGeofenceStates());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
