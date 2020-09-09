using System.Security.Cryptography.X509Certificates;
using Autofac;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Ranger.ApiUtilities;
using Ranger.Common;
using Ranger.InternalHttpClient;
using Ranger.Monitoring.HealthChecks;
using Ranger.RabbitMQ;
using Ranger.Redis;
using Ranger.Services.Breadcrumbs.Data;

namespace Ranger.Services.Breadcrumbs
{
    public class Startup
    {
        private readonly IWebHostEnvironment Environment;
        private readonly IConfiguration configuration;

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            this.Environment = environment;
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers(options =>
                 {
                     options.EnableEndpointRouting = false;
                     options.Filters.Add<OperationCanceledExceptionFilter>();
                 })
                 .AddNewtonsoftJson(options =>
                 {
                     options.SerializerSettings.Converters.Add(new StringEnumConverter());
                     options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                     options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                 });

            services.AddRangerApiVersioning();
            services.ConfigureAutoWrapperModelStateResponseFactory();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("breadcrumbsApi", policyBuilder =>
                {
                    policyBuilder.RequireScope("breadcrumbsApi");
                });
            });

            var identityAuthority = configuration["httpClient:identityAuthority"];
            services.AddPollyPolicyRegistry();
            services.AddTenantsHttpClient("http://tenants:8082", identityAuthority, "tenantsApi", "cKprgh9wYKWcsm");

            services.AddDbContext<BreadcrumbsDbContext>(options =>
            {
                options.UseNpgsql(configuration["cloudSql:ConnectionString"]);
            });

            services.AddTransient<IBreadcrumbsDbContextInitializer, BreadcrumbsDbContextInitializer>();
            services.AddTransient<ILoginRoleRepository<BreadcrumbsDbContext>, LoginRoleRepository<BreadcrumbsDbContext>>();

            services.AddRedis(configuration["redis:ConnectionString"], out _);

            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "http://identity:5000/auth";
                    options.ApiName = "breadcrumbsApi";
                    options.RequireHttpsMetadata = false;
                });

            // Workaround for MAC validation issues on MacOS
            if (configuration.IsIntegrationTesting())
            {
                services.AddDataProtection()
                   .SetApplicationName("Breadcrumbs")
                   .PersistKeysToDbContext<BreadcrumbsDbContext>();
            }
            else
            {
                services.AddDataProtection()
                    .SetApplicationName("Breadcrumbs")
                    .ProtectKeysWithCertificate(new X509Certificate2(configuration["DataProtectionCertPath:Path"]))
                    .UnprotectKeysWithAnyCertificate(new X509Certificate2(configuration["DataProtectionCertPath:Path"]))
                    .PersistKeysToDbContext<BreadcrumbsDbContext>();
            }

            services.AddLiveHealthCheck();
            services.AddEntityFrameworkHealthCheck<BreadcrumbsDbContext>();
            services.AddDockerImageTagHealthCheck();
            services.AddRabbitMQHealthCheck();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterType<TenantServiceDbContextProvider>();
            builder.RegisterInstance<CloudSqlOptions>(configuration.GetOptions<CloudSqlOptions>("cloudSql"));
            builder.Register((c, p) =>
            {
                var provider = c.Resolve<TenantServiceDbContextProvider>();
                var (dbContextOptions, _) = provider.GetDbContextOptions<BreadcrumbsDbContext>(p.TypedAs<string>());
                var breadcrumbsContext = new BreadcrumbsDbContext(dbContextOptions);
                return new BreadcrumbsRepository(breadcrumbsContext, c.Resolve<ILogger<BreadcrumbsRepository>>());
            });
            builder.AddRabbitMqWithOutbox<Startup, BreadcrumbsDbContext>();
        }

        public void Configure(IApplicationBuilder app, IHostApplicationLifetime applicationLifetime)
        {
            app.UseRouting();
            app.UseAuthentication();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks();
                endpoints.MapLiveTagHealthCheck();
                endpoints.MapEfCoreTagHealthCheck();
                endpoints.MapDockerImageTagHealthCheck();
                endpoints.MapRabbitMQHealthCheck();
            });

            app.UseRabbitMQ()
                .SubscribeCommandWithHandler<ComputeGeofenceEvents>()
                .SubscribeCommandWithHandler<InitializeTenant>((c, e) => new InitializeTenantRejected(e.Message, ""));
        }
    }
}