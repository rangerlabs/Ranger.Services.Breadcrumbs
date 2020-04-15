using Ranger.Common;

namespace Ranger.Services.Breadcrumbs
{
    public class TenantOrganizationNameModel : ContextTenant
    {
        public TenantOrganizationNameModel(string tenantId, string databasePassword, bool enabled) : base(tenantId, databasePassword, enabled)
        { }

        public string OrganizationName { get; set; }
    }
}