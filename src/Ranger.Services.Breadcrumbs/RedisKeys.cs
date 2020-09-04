namespace Ranger.Services.Breadcrumbs
{
    public static class RedisKeys
    {
        public static string TenantDbPassword(string tenantId) => $"DB_PASSWORD_{tenantId}";
    }
}