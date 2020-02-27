using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace Ranger.Services.Breadcrumbs.Data
{
    public class DesignTimeBreadcrumbDbContextFactory : IDesignTimeDbContextFactory<BreadcrumbsDbContext>
    {
        public BreadcrumbsDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var options = new DbContextOptionsBuilder<BreadcrumbsDbContext>();
            options.UseNpgsql(config["cloudSql:ConnectionString"]);

            return new BreadcrumbsDbContext(options.Options);
        }
    }
}