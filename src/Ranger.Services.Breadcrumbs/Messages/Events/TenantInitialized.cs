using Ranger.RabbitMQ;

namespace Ranger.Services.Breadcrumbs
{
    [MessageNamespaceAttribute("breadcrumbs")]
    public class TenantInitialized : IEvent
    {
        public TenantInitialized() { }
    }
}