using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ranger.Common;
using Ranger.InternalHttpClient;

namespace Ranger.Services.Breadcrumbs.Tests.IntegrationTests
{
    public class TestTenantsHttpClient : ITenantsHttpClient
    {
        private readonly ILogger<TestTenantsHttpClient> logger;

        public TestTenantsHttpClient(HttpClient client, ILogger<TestTenantsHttpClient> logger)
        {
            this.logger = logger;
        }

        public Task<RangerApiResponse<T>> GetTenantByIdAsync<T>(string tenantId, CancellationToken cancellationToken = default)
            where T : class
        {
            logger.LogInformation("Calling from Test client");
            var response = new RangerApiResponse<T>();
            response.Result = new ContextTenant(tenantId, Constants.TenantPassword, true) as T;
            return Task.FromResult(response);
        }

        Task<RangerApiResponse> ITenantsHttpClient.ConfirmTenantAsync(string domain, string jsonContent, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        Task<RangerApiResponse<bool>> ITenantsHttpClient.DoesExistAsync(string domain, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        Task<RangerApiResponse<T>> ITenantsHttpClient.GetAllTenantsAsync<T>(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        Task<RangerApiResponse<T>> ITenantsHttpClient.GetPrimaryOwnerTransferByDomain<T>(string domain, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        Task<RangerApiResponse<T>> ITenantsHttpClient.GetTenantByDomainAsync<T>(string domain, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        Task<RangerApiResponse<bool>> ITenantsHttpClient.IsConfirmedAsync(string domain, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}