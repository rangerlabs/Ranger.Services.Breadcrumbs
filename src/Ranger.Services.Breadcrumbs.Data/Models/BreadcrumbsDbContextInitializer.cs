using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Ranger.Common;

namespace Ranger.Services.Breadcrumbs.Data
{
    public class BreadcrumbsDbContextInitializer : IBreadcrumbsDbContextInitializer
    {
        private readonly BreadcrumbsDbContext context;

        public BreadcrumbsDbContextInitializer(BreadcrumbsDbContext context)
        {
            this.context = context;
        }

        public bool EnsureCreated()
        {
            return context.Database.EnsureCreated();
        }

        public void Migrate()
        {
            context.Database.Migrate();
        }

        public async Task EnsureRowLevelSecurityApplied()
        {
            var tables = Enum.GetNames(typeof(RowLevelSecureTablesEnum));
            var loginRoleRepository = new LoginRoleRepository<BreadcrumbsDbContext>(context);
            foreach (var table in tables)
            {
                await loginRoleRepository.CreateTenantRlsPolicy(table);
            }
        }
    }

    public interface IBreadcrumbsDbContextInitializer
    {
        bool EnsureCreated();
        void Migrate();
        Task EnsureRowLevelSecurityApplied();
    }
}