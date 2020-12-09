using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Ranger.Common;
using Ranger.RabbitMQ;
using Ranger.RabbitMQ.BusPublisher;
using Ranger.Services.Breadcrumbs.Data;

namespace Ranger.Services.Breadcrumbs.Handlers
{
    public class ComputeGeofenceEventsHandler : ICommandHandler<ComputeGeofenceEvents>
    {
        private readonly IBusPublisher busPublisher;
        private readonly Func<string, BreadcrumbsRepository> breadcrumbsRepoFactory;
        private readonly ILogger<ComputeGeofenceEventsHandler> logger;

        public ComputeGeofenceEventsHandler(IBusPublisher busPublisher, Func<string, BreadcrumbsRepository> breadcrumbsRepoFactory, ILogger<ComputeGeofenceEventsHandler> logger)
        {
            this.busPublisher = busPublisher;
            this.breadcrumbsRepoFactory = breadcrumbsRepoFactory;
            this.logger = logger;
        }

        public async Task HandleAsync(ComputeGeofenceEvents message, ICorrelationContext context)
        {
            var breadcrumbsRepo = breadcrumbsRepoFactory(message.TenantId);

            IList<ConcurrentBreadcrumbResult> concurrentBreadcrumbResults = new List<ConcurrentBreadcrumbResult>();
            try
            {
                concurrentBreadcrumbResults = await breadcrumbsRepo.UpsertGeofenceStates(message.TenantId, message.ProjectId, message.Breadcrumb.DeviceId, message.GeofenceIntersectionIds, message.Breadcrumb.RecordedAt);
            }
            catch (RangerException)
            { }

            if (!concurrentBreadcrumbResults.Any())
            {
                concurrentBreadcrumbResults.Add(new ConcurrentBreadcrumbResult(message.ProjectId, message.Breadcrumb.DeviceId, Guid.Empty, GeofenceEventEnum.NONE));
            }

            // disgusted with these mappings
            var breadcrumbGeofenceResults = concurrentBreadcrumbResults.Select(_ => new BreadcrumbGeofenceResult { TenantId = message.TenantId, GeofenceId = _.GeofenceId, GeofenceEvent = _.LastEvent }).ToList();

            var breadcrumb = new Data.Breadcrumb
            {
                TenantId = message.TenantId,
                ProjectId = message.ProjectId,
                Environment = message.Environment,
                DeviceId = message.Breadcrumb.DeviceId,
                ExternalUserId = message.Breadcrumb.ExternalUserId,
                Position = message.Breadcrumb.Position,
                Accuracy = message.Breadcrumb.Accuracy,
                RecordedAt = message.Breadcrumb.RecordedAt,
                AcceptedAt = message.Breadcrumb.AcceptedAt
            };

            var id = await breadcrumbsRepo.AddBreadcrumbAndBreadcrumbGeofenceResults(breadcrumb, breadcrumbGeofenceResults);
            var identifiedBreadcrumb = new Common.Breadcrumb(message.Breadcrumb.DeviceId, message.Breadcrumb.ExternalUserId, message.Breadcrumb.Position, message.Breadcrumb.RecordedAt, message.Breadcrumb.AcceptedAt, message.Breadcrumb.Metadata, message.Breadcrumb.Accuracy, id);

            busPublisher.Send(new ComputeGeofenceIntegrations(message.TenantId, message.ProjectId, message.ProjectName, message.Environment, identifiedBreadcrumb, breadcrumbGeofenceResults), context);

        }
    }
}