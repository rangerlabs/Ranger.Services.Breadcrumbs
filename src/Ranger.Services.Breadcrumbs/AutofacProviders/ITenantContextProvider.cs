using Microsoft.EntityFrameworkCore;
using Ranger.Services.Breadcrumbs.Data;

namespace Ranger.Services.Breadcrumbs
{
    public interface ITenantContextProvider
    {
        (DbContextOptions<BreadcrumbsDbContext> options, TenantOrganizationNameModel databaseUsername) GetDbContextOptions(string tenant);
    }
}