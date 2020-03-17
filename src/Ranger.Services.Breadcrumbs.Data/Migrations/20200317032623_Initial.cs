﻿using System;
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
                    geofence_id = table.Column<Guid>(nullable: false),
                    geofence_event = table.Column<int>(nullable: false),
                    device_id = table.Column<string>(nullable: false),
                    external_user_id = table.Column<string>(nullable: false),
                    position = table.Column<string>(nullable: false),
                    accuracy = table.Column<double>(nullable: false),
                    recorded_at = table.Column<DateTime>(nullable: false),
                    metadata = table.Column<string>(type: "jsonb", nullable: true),
                    correlated_entered_event_id = table.Column<int>(nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_breadcrumbs_environment",
                table: "breadcrumbs",
                column: "environment");

            migrationBuilder.CreateIndex(
                name: "IX_breadcrumbs_project_id",
                table: "breadcrumbs",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_breadcrumbs_geofence_id_device_id",
                table: "breadcrumbs",
                columns: new[] { "geofence_id", "device_id" });

            migrationBuilder.CreateIndex(
                name: "IX_breadcrumbs_geofence_id_external_user_id",
                table: "breadcrumbs",
                columns: new[] { "geofence_id", "external_user_id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "breadcrumbs");

            migrationBuilder.DropTable(
                name: "data_protection_keys");
        }
    }
}
