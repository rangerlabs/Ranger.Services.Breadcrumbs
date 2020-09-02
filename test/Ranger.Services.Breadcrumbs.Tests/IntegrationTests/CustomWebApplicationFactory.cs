using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using Ranger.InternalHttpClient;
using Ranger.Services.Breadcrumbs.Data;

namespace Ranger.Services.Breadcrumbs.Tests.IntegrationTests
{
    public class CustomWebApplicationFactory
        : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment(Environments.Production);

            builder.ConfigureAppConfiguration((context, conf) =>
            {
                conf.SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .AddEnvironmentVariables();
            });

            builder.ConfigureServices(services =>
            {
                var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

                services.AddDbContext<BreadcrumbsDbContext>(options =>
                     {
                         options.UseNpgsql(configuration["cloudSql:ConnectionString"]);
                     });

                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<BreadcrumbsDbContext>();
                    context.Database.Migrate();
                }
            });

            builder.ConfigureTestServices(services =>
            {
                var serviceDescriptors = services.Where(x => x.ServiceType == typeof(ITenantsHttpClient)).ToList();
                foreach (var serviceDescriptor in serviceDescriptors)
                {
                    services.Remove(serviceDescriptor);
                }
                services.AddHttpClient<ITenantsHttpClient, TestTenantsHttpClient>();
            });
        }
    }
}