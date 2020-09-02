using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Ranger.Common;
using Ranger.RabbitMQ;
using Ranger.RabbitMQ.BusPublisher;
using Ranger.RabbitMQ.BusSubscriber;
using Ranger.Services.Breadcrumbs.Data;
using Shouldly;
using Xunit;

namespace Ranger.Services.Breadcrumbs.Tests.IntegrationTests
{
    public class HandlerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly IBusPublisher busPublisher;
        private readonly IBusSubscriber busSubscriber;
        private readonly CustomWebApplicationFactory _factory;
        private readonly BreadcrumbsDbContext _context;
        private readonly IConfiguration _configuration;

        public HandlerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _context = factory.Services.GetService(typeof(BreadcrumbsDbContext)) as BreadcrumbsDbContext;
            _configuration = factory.Services.GetService(typeof(IConfiguration)) as IConfiguration;
            this.busPublisher = factory.Services.GetService(typeof(IBusPublisher)) as IBusPublisher;
            this.busSubscriber = factory.Services.GetService(typeof(IBusSubscriber)) as IBusSubscriber;
        }

        [Fact]
        public void Breadcrumbs_Starts()
        { }

        [Fact]
        public async Task InitializeTenantHandler_Sends_TenantInitialized()
        {
            var msg = new InitializeTenant(Constants.TenantId, Constants.TenantPassword);
            var handled = false;
            TenantInitialized response = null;
            busSubscriber.SubscribeEventWithCallback<TenantInitialized>((m, c) =>
            {
                handled = true;
                response = m;
                return Task.CompletedTask;
            });
            busPublisher.Send(msg, CorrelationContext.Empty);

            while (!handled) { }

            using var connection = new NpgsqlConnection(_configuration["cloudSql:ConnectionString"]);
            using var cmd = new NpgsqlCommand("SELECT EXISTS (SELECT usename FROM pg_catalog.pg_user WHERE usename = @p)", connection);
            cmd.Parameters.AddWithValue("@p", msg.TenantId);

            await connection.OpenAsync();
            using var result = await cmd.ExecuteReaderAsync();
            result.Read();
            result.GetBoolean(0).ShouldBeTrue();
        }

        // [Fact]
        // public async Task Breadcrumbs_Shutsdown_Gracefully()
        // {

        //     int runs = 100;
        //     for (int i = 0; i < runs; i++)
        //     {
        //         if (i == runs / 2)
        //         {
        //             await _factory.Server.Host.StopAsync();
        //         }
        //         busPublisher.Send(new ComputeGeofenceEvents(Constants.TenantId, Constants.ProjectId, "project", EnvironmentEnum.TEST,
        //             new Common.Breadcrumb(
        //                 "Ranger_Test_Runner",
        //                 "Test_User_0",
        //                 new LngLat(-81.55693137783203, 41.487167846074094),
        //                 DateTime.Now,
        //                 DateTime.UtcNow,
        //                 default,
        //                 0),
        //                 Array.Empty<Guid>()), CorrelationContext.Empty);
        //     }
        // }
    }
}