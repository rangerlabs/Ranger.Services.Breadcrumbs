using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Ranger.Services.Breadcrumbs.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "breadcrumbs",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    database_username = table.Column<string>(nullable: false),
                    project_id = table.Column<Guid>(nullable: false),
                    environment = table.Column<int>(nullable: false),
                    device_id = table.Column<string>(nullable: false),
                    external_user_id = table.Column<string>(nullable: true),
                    position = table.Column<string>(type: "jsonb", nullable: false),
                    accuracy = table.Column<double>(nullable: false),
                    recorded_at = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_breadcrumbs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "data_protection_keys",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    friendly_name = table.Column<string>(nullable: true),
                    xml = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_data_protection_keys", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "breadcrumb_geofence_results",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    breadcrumb_id = table.Column<int>(nullable: false),
                    entered_breadcrumb_id = table.Column<int>(nullable: true),
                    geofence_id = table.Column<Guid>(nullable: false),
                    geofence_event = table.Column<int>(nullable: false),
                    database_username = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_breadcrumb_geofence_results", x => x.id);
                    table.ForeignKey(
                        name: "FK_breadcrumb_geofence_results_breadcrumbs_breadcrumb_id",
                        column: x => x.breadcrumb_id,
                        principalTable: "breadcrumbs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_breadcrumb_geofence_results_breadcrumbs_entered_breadcrumb_~",
                        column: x => x.entered_breadcrumb_id,
                        principalTable: "breadcrumbs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "not_exited_breadcrumb_states",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    project_id = table.Column<Guid>(nullable: false),
                    breadcrumb_id = table.Column<int>(nullable: false),
                    device_id = table.Column<string>(nullable: false),
                    database_username = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_not_exited_breadcrumb_states", x => x.id);
                    table.ForeignKey(
                        name: "fk_not_exited_breadcrumb_states_breadcrumbs_breadcrumb_id",
                        column: x => x.breadcrumb_id,
                        principalTable: "breadcrumbs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_breadcrumb_geofence_results_breadcrumb_id",
                table: "breadcrumb_geofence_results",
                column: "breadcrumb_id");

            migrationBuilder.CreateIndex(
                name: "IX_breadcrumb_geofence_results_entered_breadcrumb_id",
                table: "breadcrumb_geofence_results",
                column: "entered_breadcrumb_id");

            migrationBuilder.CreateIndex(
                name: "IX_breadcrumb_geofence_results_geofence_event",
                table: "breadcrumb_geofence_results",
                column: "geofence_event");

            migrationBuilder.CreateIndex(
                name: "IX_breadcrumb_geofence_results_geofence_id",
                table: "breadcrumb_geofence_results",
                column: "geofence_id");

            migrationBuilder.CreateIndex(
                name: "IX_breadcrumbs_environment",
                table: "breadcrumbs",
                column: "environment");

            migrationBuilder.CreateIndex(
                name: "IX_breadcrumbs_project_id",
                table: "breadcrumbs",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "ix_not_exited_breadcrumb_states_breadcrumb_id",
                table: "not_exited_breadcrumb_states",
                column: "breadcrumb_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_not_exited_breadcrumb_states_device_id",
                table: "not_exited_breadcrumb_states",
                column: "device_id");

            migrationBuilder.CreateIndex(
                name: "IX_not_exited_breadcrumb_states_project_id",
                table: "not_exited_breadcrumb_states",
                column: "project_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "breadcrumb_geofence_results");

            migrationBuilder.DropTable(
                name: "data_protection_keys");

            migrationBuilder.DropTable(
                name: "not_exited_breadcrumb_states");

            migrationBuilder.DropTable(
                name: "breadcrumbs");
        }
    }
}
