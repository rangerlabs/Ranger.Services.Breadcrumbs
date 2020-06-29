using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ranger.Services.Breadcrumbs.Data.Migrations
{
    public partial class AddAcceptedAtTimestamp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "accepted_at",
                table: "breadcrumbs",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "accepted_at",
                table: "breadcrumbs");
        }
    }
}
