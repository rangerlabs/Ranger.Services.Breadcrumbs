﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Ranger.Services.Breadcrumbs.Data;

namespace Ranger.Services.Breadcrumbs.Data.Migrations
{
    [DbContext(typeof(BreadcrumbsDbContext))]
    [Migration("20200910135007_AddOutboxMsgTypeVersion")]
    partial class AddOutboxMsgTypeVersion
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Microsoft.AspNetCore.DataProtection.EntityFrameworkCore.DataProtectionKey", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("FriendlyName")
                        .HasColumnName("friendly_name")
                        .HasColumnType("text");

                    b.Property<string>("Xml")
                        .HasColumnName("xml")
                        .HasColumnType("text");

                    b.HasKey("Id")
                        .HasName("pk_data_protection_keys");

                    b.ToTable("data_protection_keys");
                });

            modelBuilder.Entity("Ranger.RabbitMQ.OutboxMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("InsertedAt")
                        .HasColumnName("inserted_at")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("MessageId")
                        .HasColumnName("message_id")
                        .HasColumnType("integer");

                    b.Property<bool>("Nacked")
                        .HasColumnName("nacked")
                        .HasColumnType("boolean");

                    b.HasKey("Id")
                        .HasName("pk_outbox");

                    b.HasIndex("MessageId")
                        .HasName("ix_outbox_message_id");

                    b.ToTable("outbox");
                });

            modelBuilder.Entity("Ranger.RabbitMQ.RangerRabbitMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Body")
                        .IsRequired()
                        .HasColumnName("body")
                        .HasColumnType("text");

                    b.Property<string>("Headers")
                        .IsRequired()
                        .HasColumnName("headers")
                        .HasColumnType("text");

                    b.Property<float>("MessageVersion")
                        .HasColumnName("message_version")
                        .HasColumnType("real");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnName("type")
                        .HasColumnType("text");

                    b.HasKey("Id")
                        .HasName("pk_ranger_rabbit_message");

                    b.ToTable("ranger_rabbit_message");
                });

            modelBuilder.Entity("Ranger.Services.Breadcrumbs.Data.BreadcrumbEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("AcceptedAt")
                        .HasColumnName("accepted_at")
                        .HasColumnType("timestamp without time zone");

                    b.Property<double>("Accuracy")
                        .HasColumnName("accuracy")
                        .HasColumnType("double precision");

                    b.Property<string>("DeviceId")
                        .IsRequired()
                        .HasColumnName("device_id")
                        .HasColumnType("text");

                    b.Property<int>("Environment")
                        .HasColumnName("environment")
                        .HasColumnType("integer");

                    b.Property<string>("ExternalUserId")
                        .HasColumnName("external_user_id")
                        .HasColumnType("text");

                    b.Property<string>("Position")
                        .IsRequired()
                        .HasColumnName("position")
                        .HasColumnType("jsonb");

                    b.Property<Guid>("ProjectId")
                        .HasColumnName("project_id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("RecordedAt")
                        .HasColumnName("recorded_at")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("TenantId")
                        .IsRequired()
                        .HasColumnName("tenant_id")
                        .HasColumnType("text");

                    b.HasKey("Id")
                        .HasName("pk_breadcrumbs");

                    b.HasIndex("Environment");

                    b.HasIndex("ProjectId");

                    b.ToTable("breadcrumbs");
                });

            modelBuilder.Entity("Ranger.Services.Breadcrumbs.Data.BreadcrumbGeofenceResult", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("BreadcrumbId")
                        .HasColumnName("breadcrumb_id")
                        .HasColumnType("integer");

                    b.Property<int?>("EnteredBreadcrumbId")
                        .HasColumnName("entered_breadcrumb_id")
                        .HasColumnType("integer");

                    b.Property<int>("GeofenceEvent")
                        .HasColumnName("geofence_event")
                        .HasColumnType("integer");

                    b.Property<Guid>("GeofenceId")
                        .HasColumnName("geofence_id")
                        .HasColumnType("uuid");

                    b.Property<string>("TenantId")
                        .IsRequired()
                        .HasColumnName("tenant_id")
                        .HasColumnType("text");

                    b.HasKey("Id")
                        .HasName("pk_breadcrumb_geofence_results");

                    b.HasIndex("BreadcrumbId");

                    b.HasIndex("EnteredBreadcrumbId");

                    b.HasIndex("GeofenceEvent");

                    b.HasIndex("GeofenceId");

                    b.ToTable("breadcrumb_geofence_results");
                });

            modelBuilder.Entity("Ranger.Services.Breadcrumbs.Data.NotExitedBreadcrumbState", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("BreadcrumbId")
                        .HasColumnName("breadcrumb_id")
                        .HasColumnType("integer");

                    b.Property<string>("DeviceId")
                        .IsRequired()
                        .HasColumnName("device_id")
                        .HasColumnType("text");

                    b.Property<Guid>("ProjectId")
                        .HasColumnName("project_id")
                        .HasColumnType("uuid");

                    b.Property<string>("TenantId")
                        .IsRequired()
                        .HasColumnName("tenant_id")
                        .HasColumnType("text");

                    b.HasKey("Id")
                        .HasName("pk_not_exited_breadcrumb_states");

                    b.HasIndex("BreadcrumbId")
                        .IsUnique()
                        .HasName("ix_not_exited_breadcrumb_states_breadcrumb_id");

                    b.HasIndex("DeviceId");

                    b.HasIndex("ProjectId");

                    b.ToTable("not_exited_breadcrumb_states");
                });

            modelBuilder.Entity("Ranger.RabbitMQ.OutboxMessage", b =>
                {
                    b.HasOne("Ranger.RabbitMQ.RangerRabbitMessage", "Message")
                        .WithMany()
                        .HasForeignKey("MessageId")
                        .HasConstraintName("fk_outbox_ranger_rabbit_message_message_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Ranger.Services.Breadcrumbs.Data.BreadcrumbGeofenceResult", b =>
                {
                    b.HasOne("Ranger.Services.Breadcrumbs.Data.BreadcrumbEntity", "Breadcrumb")
                        .WithMany("BreadcrumbGeofenceResults")
                        .HasForeignKey("BreadcrumbId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Ranger.Services.Breadcrumbs.Data.BreadcrumbEntity", "EnteredBreadcrumb")
                        .WithMany("EnteredBreadcrumbGeofenceResults")
                        .HasForeignKey("EnteredBreadcrumbId");
                });

            modelBuilder.Entity("Ranger.Services.Breadcrumbs.Data.NotExitedBreadcrumbState", b =>
                {
                    b.HasOne("Ranger.Services.Breadcrumbs.Data.BreadcrumbEntity", "Breadcrumb")
                        .WithOne("UnexitedEnteredBreadcrumb")
                        .HasForeignKey("Ranger.Services.Breadcrumbs.Data.NotExitedBreadcrumbState", "BreadcrumbId")
                        .HasConstraintName("fk_not_exited_breadcrumb_states_breadcrumbs_breadcrumb_id");
                });
#pragma warning restore 612, 618
        }
    }
}
