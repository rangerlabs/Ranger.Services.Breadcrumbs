using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ranger.Services.Breadcrumbs.Data
{
    public interface IBreadcrumbsRepository
    {
        Task AddBreadcrumb(Breadcrumb breadcrumb);
        Task<IEnumerable<(BreadcrumbGeofenceResult, int)>> GetUserOrDeviceCurrentlyEnteredBreadcrumbs(Ranger.Common.Breadcrumb breadcrumb, Guid projectId, IEnumerable<Guid> geofenceIds);
        Task RemoveUnexitedEnteredBreadcrumbIds(IEnumerable<int> unExitedEnteredBreadcrumbIds);
    }
}