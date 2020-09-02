using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Ranger.Services.Breadcrumbs.Tests.IntegrationTests
{
    public static class Extensions
    {
        public static void SwapTransient<TService, TImplementation>(this IServiceCollection services)
    where TImplementation : class, TService
        {


            services.AddTransient(typeof(TService), typeof(TImplementation));
        }
    }
}