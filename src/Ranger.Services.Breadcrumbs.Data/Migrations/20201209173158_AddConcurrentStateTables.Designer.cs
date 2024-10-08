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
    [Migration("20201209173158_AddConcurrentStateTables")]
    partial class AddConcurrentStateTables
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
                        .HasName("pk_outbox_messages");

                    b.HasIndex("MessageId")
                        .IsUnique()
                        .HasName("ix_outbox_messages_message_id");

                    b.ToTable("outbox_messages");
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
                        .HasName("pk_ranger_rabbit_messages");

                    b.ToTable("ranger_rabbit_messages");
                });

            modelBuilder.Entity("Ranger.Services.Breadcrumbs.Data.BreadcrumbEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("bigint")
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
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<long>("BreadcrumbId")
                        .HasColumnName("breadcrumb_id")
                        .HasColumnType("bigint");

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

                    b.HasIndex("BreadcrumbId")
                        .HasName("ix_breadcrumb_geofence_results_breadcrumb_id");

                    b.HasIndex("GeofenceEvent");

                    b.HasIndex("GeofenceId");

                    b.ToTable("breadcrumb_geofence_results");
                });

            modelBuilder.Entity("Ranger.Services.Breadcrumbs.Data.DeviceGeofenceState", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("DeviceId")
                        .IsRequired()
                        .HasColumnName("device_id")
                        .HasColumnType("text");

                    b.Property<Guid>("GeofenceId")
                        .HasColumnName("geofence_id")
                        .HasColumnType("uuid");

                    b.Property<int>("LastEvent")
                        .HasColumnName("last_event")
                        .HasColumnType("integer");

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
                        .HasName("pk_device_geofence_states");

                    b.HasIndex("ProjectId", "GeofenceId", "DeviceId")
                        .IsUnique();

                    b.ToTable("device_geofence_states");
                });

            modelBuilder.Entity("Ranger.Services.Breadcrumbs.Data.LastDeviceRecordedAt", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("DeviceId")
                        .IsRequired()
                        .HasColumnName("device_id")
                        .HasColumnType("text");

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
                        .HasName("pk_last_device_recorded_ats");

                    b.HasIndex("ProjectId", "DeviceId")
                        .IsUnique();

                    b.ToTable("last_device_recorded_ats");
                });

            modelBuilder.Entity("Ranger.RabbitMQ.OutboxMessage", b =>
                {
                    b.HasOne("Ranger.RabbitMQ.RangerRabbitMessage", "Message")
                        .WithOne("OutboxMessage")
                        .HasForeignKey("Ranger.RabbitMQ.OutboxMessage", "MessageId")
                        .HasConstraintName("fk_outbox_messages_ranger_rabbit_messages_message_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Ranger.Services.Breadcrumbs.Data.BreadcrumbGeofenceResult", b =>
                {
                    b.HasOne("Ranger.Services.Breadcrumbs.Data.BreadcrumbEntity", "Breadcrumb")
                        .WithMany("BreadcrumbGeofenceResults")
                        .HasForeignKey("BreadcrumbId")
                        .HasConstraintName("fk_breadcrumb_geofence_results_breadcrumbs_breadcrumb_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
