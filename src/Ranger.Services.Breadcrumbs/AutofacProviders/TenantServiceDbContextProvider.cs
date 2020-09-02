using System;
using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Ranger.Common;
using Ranger.InternalHttpClient;

namespace Ranger.Services.Breadcrumbs
{
    public class TenantServiceDbContextProvider
    {
        private readonly ITenantsHttpClient tenantsClient;
        private readonly ILogger<TenantServiceDbContextProvider> logger;
        private readonly CloudSqlOptions cloudSqlOptions;

        public TenantServiceDbContextProvider(ITenantsHttpClient tenantsClient, CloudSqlOptions cloudSqlOptions, ILogger<TenantServiceDbContextProvider> logger)
        {
            this.cloudSqlOptions = cloudSqlOptions;
            this.logger = logger;
            this.tenantsClient = tenantsClient;
        }

        public (DbContextOptions<T> options, ContextTenant contextTenant) GetDbContextOptions<T>(string tenantId)
            where T : DbContext
        {
            var apiResponse = tenantsClient.GetTenantByIdAsync<ContextTenant>(tenantId).Result;
            NpgsqlConnectionStringBuilder connectionBuilder = new NpgsqlConnectionStringBuilder(cloudSqlOptions.ConnectionString);
            connectionBuilder.Username = tenantId;
            connectionBuilder.Password = apiResponse.Result.DatabasePassword;

            var options = new DbContextOptionsBuilder<T>();
            options.UseNpgsql(connectionBuilder.ToString());
            return (options.Options, apiResponse.Result);
        }
    }
}