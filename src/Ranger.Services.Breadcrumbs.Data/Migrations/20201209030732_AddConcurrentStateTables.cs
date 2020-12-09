using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Ranger.Services.Breadcrumbs.Data.Migrations
{
    public partial class AddConcurrentStateTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_breadcrumb_geofence_results_breadcrumbs_breadcrumb_id",
                table: "breadcrumb_geofence_results");

            migrationBuilder.DropForeignKey(
                name: "FK_breadcrumb_geofence_results_breadcrumbs_entered_breadcrumb_~",
                table: "breadcrumb_geofence_results");

            migrationBuilder.DropTable(
                name: "not_exited_breadcrumb_states");

            migrationBuilder.DropIndex(
                name: "IX_breadcrumb_geofence_results_entered_breadcrumb_id",
                table: "breadcrumb_geofence_results");

            migrationBuilder.DropColumn(
                name: "entered_breadcrumb_id",
                table: "breadcrumb_geofence_results");

            migrationBuilder.RenameIndex(
                name: "IX_breadcrumb_geofence_results_breadcrumb_id",
                table: "breadcrumb_geofence_results",
                newName: "ix_breadcrumb_geofence_results_breadcrumb_id");

            migrationBuilder.AlterColumn<long>(
                name: "id",
                table: "breadcrumbs",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<long>(
                name: "breadcrumb_id",
                table: "breadcrumb_geofence_results",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "id",
                table: "breadcrumb_geofence_results",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateTable(
                name: "device_geofence_states",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    project_id = table.Column<Guid>(nullable: false),
                    device_id = table.Column<string>(nullable: false),
                    recorded_at = table.Column<DateTime>(nullable: false),
                    last_event = table.Column<int>(nullable: false),
                    geofence_id = table.Column<Guid>(nullable: false),
                    tenant_id = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_device_geofence_states", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "last_device_recorded_ats",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    project_id = table.Column<Guid>(nullable: false),
                    device_id = table.Column<string>(nullable: false),
                    recorded_at = table.Column<DateTime>(nullable: false),
                    tenant_id = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_last_device_recorded_ats", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_device_geofence_states_project_id_geofence_id_device_id",
                table: "device_geofence_states",
                columns: new[] { "project_id", "geofence_id", "device_id" });

            migrationBuilder.CreateIndex(
                name: "IX_last_device_recorded_ats_project_id_device_id",
                table: "last_device_recorded_ats",
                columns: new[] { "project_id", "device_id" });

            migrationBuilder.AddForeignKey(
                name: "fk_breadcrumb_geofence_results_breadcrumbs_breadcrumb_id",
                table: "breadcrumb_geofence_results",
                column: "breadcrumb_id",
                principalTable: "breadcrumbs",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_breadcrumb_geofence_results_breadcrumbs_breadcrumb_id",
                table: "breadcrumb_geofence_results");

            migrationBuilder.DropTable(
                name: "device_geofence_states");

            migrationBuilder.DropTable(
                name: "last_device_recorded_ats");

            migrationBuilder.RenameIndex(
                name: "ix_breadcrumb_geofence_results_breadcrumb_id",
                table: "breadcrumb_geofence_results",
                newName: "IX_breadcrumb_geofence_results_breadcrumb_id");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "breadcrumbs",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "breadcrumb_id",
                table: "breadcrumb_geofence_results",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "breadcrumb_geofence_results",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "entered_breadcrumb_id",
                table: "breadcrumb_geofence_results",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "not_exited_breadcrumb_states",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    breadcrumb_id = table.Column<int>(type: "integer", nullable: false),
                    device_id = table.Column<string>(type: "text", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<string>(type: "text", nullable: false)
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
                name: "IX_breadcrumb_geofence_results_entered_breadcrumb_id",
                table: "breadcrumb_geofence_results",
                column: "entered_breadcrumb_id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_breadcrumb_geofence_results_breadcrumbs_breadcrumb_id",
                table: "breadcrumb_geofence_results",
                column: "breadcrumb_id",
                principalTable: "breadcrumbs",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_breadcrumb_geofence_results_breadcrumbs_entered_breadcrumb_~",
                table: "breadcrumb_geofence_results",
                column: "entered_breadcrumb_id",
                principalTable: "breadcrumbs",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
