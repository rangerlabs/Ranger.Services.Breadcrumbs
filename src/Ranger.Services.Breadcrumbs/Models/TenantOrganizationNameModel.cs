using Ranger.Common;

namespace Ranger.Services.Breadcrumbs
{
    public class TenantOrganizationNameModel : ContextTenant
    {
        public TenantOrganizationNameModel(string databaseUsername, string databasePassword, bool enabled) : base(databaseUsername, databasePassword, enabled)
        { }

        public string OrganizationName { get; set; }
    }
}