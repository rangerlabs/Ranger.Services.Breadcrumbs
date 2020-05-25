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
using Newtonsoft.Json.Serialization;
using Ranger.Common;
using Ranger.InternalHttpClient;
using Ranger.Monitoring.HealthChecks;
using Ranger.RabbitMQ;
using Ranger.Services.Breadcrumbs.Data;

namespace Ranger.Services.Breadcrumbs
{
    public class Startup
    {
        private readonly IWebHostEnvironment Environment;
        private readonly IConfiguration configuration;
        private ILoggerFactory loggerFactory;
        private IBusSubscriber busSubscriber;

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
                 })
                 .AddNewtonsoftJson(options =>
                 {
                     options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                     options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                 });

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
            services.AddProjectsHttpClient("http://projects:8086", identityAuthority, "projectsApi", "usGwT8Qsp4La2");
            services.AddIdentityHttpClient("http://identity:5000", identityAuthority, "IdentityServerApi", "89pCcXHuDYTXY");

            services.AddDbContext<BreadcrumbsDbContext>(options =>
            {
                options.UseNpgsql(configuration["cloudSql:ConnectionString"]);
            },
                ServiceLifetime.Transient
            );

            services.AddTransient<IBreadcrumbsDbContextInitializer, BreadcrumbsDbContextInitializer>();
            services.AddTransient<ILoginRoleRepository<BreadcrumbsDbContext>, LoginRoleRepository<BreadcrumbsDbContext>>();

            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "http://identity:5000/auth";
                    options.ApiName = "breadcrumbsApi";
                    options.RequireHttpsMetadata = false;
                });

            services.AddDataProtection()
                .SetApplicationName("Breadcrumbs")
                .ProtectKeysWithCertificate(new X509Certificate2(configuration["DataProtectionCertPath:Path"]))
                .UnprotectKeysWithAnyCertificate(new X509Certificate2(configuration["DataProtectionCertPath:Path"]))
                .PersistKeysToDbContext<BreadcrumbsDbContext>();

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
            builder.AddRabbitMq();
        }

        public void Configure(IApplicationBuilder app, IHostApplicationLifetime applicationLifetime, ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;

            applicationLifetime.ApplicationStopping.Register(OnShutdown);

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

            this.busSubscriber = app.UseRabbitMQ()
                .SubscribeCommand<ComputeGeofenceEvents>()
                .SubscribeCommand<InitializeTenant>((c, e) => new InitializeTenantRejected(e.Message, ""));
        }

        private void OnShutdown()
        {
            this.busSubscriber.Dispose();
        }
    }
}