using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ranger.Services.Breadcrumbs.Data
{
    public interface IBreadcrumbsRepository
    {
        Task<long> AddBreadcrumbAndBreadcrumbGeofenceResults(Data.Breadcrumb breadcrumb, ICollection<BreadcrumbGeofenceResult> results);
        Task<IList<ConcurrentBreadcrumbResult>> UpsertGeofenceStates(string tenantId, Guid projectId, string deviceId, IEnumerable<Guid> geofenceIds, DateTime recordedAt);
    }
}