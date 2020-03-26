using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Ranger.Common;
using Ranger.InternalHttpClient;
using Ranger.Services.Breadcrumbs.Data;

namespace Ranger.Services.Breadcrumbs
{
    public class TenantServiceBreadcrumbsDbContextProvider : ITenantContextProvider
    {
        private readonly ITenantsClient tenantsClient;
        private readonly ILogger<TenantServiceBreadcrumbsDbContextProvider> logger;
        private readonly CloudSqlOptions cloudSqlOptions;

        public TenantServiceBreadcrumbsDbContextProvider(ITenantsClient tenantsClient, CloudSqlOptions cloudSqlOptions, ILogger<TenantServiceBreadcrumbsDbContextProvider> logger)
        {
            this.cloudSqlOptions = cloudSqlOptions;
            this.logger = logger;
            this.tenantsClient = tenantsClient;
        }

        public (DbContextOptions<BreadcrumbsDbContext> options, TenantOrganizationNameModel databaseUsername) GetDbContextOptions(string tenant)
        {
            TenantOrganizationNameModel contextTenant = null;
            try
            {
                contextTenant = this.tenantsClient.GetTenantAsync<TenantOrganizationNameModel>(tenant).Result;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "An exception occurred retrieving the ContextTenant object. Cannot construct the tenant specific repository.");
                throw;
            }

            NpgsqlConnectionStringBuilder connectionBuilder = new NpgsqlConnectionStringBuilder(cloudSqlOptions.ConnectionString);
            connectionBuilder.Username = contextTenant.DatabaseUsername;
            connectionBuilder.Password = contextTenant.DatabasePassword;

            var options = new DbContextOptionsBuilder<BreadcrumbsDbContext>();
            options.UseNpgsql(connectionBuilder.ToString());
            return (options.Options, contextTenant);
        }

    }
}