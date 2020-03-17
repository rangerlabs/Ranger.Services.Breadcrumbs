using System.Threading.Tasks;
using Ranger.RabbitMQ;
using Ranger.Services.Breadcrumbs.Data;

namespace Ranger.Services.Breadcrumbs.Handlers
{
    public class GeofenceIntersectionComputedHandler : IEventHandler<GeofenceIntersectionsComputed>
    {
        private readonly BreadcrumbsDbContext context;

        public GeofenceIntersectionComputedHandler(BreadcrumbsDbContext context)
        {
            this.context = context;
        }

        public Task HandleAsync(GeofenceIntersectionsComputed message, ICorrelationContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}