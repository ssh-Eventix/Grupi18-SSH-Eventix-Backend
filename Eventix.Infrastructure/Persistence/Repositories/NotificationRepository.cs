using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;

namespace Eventix.Infrastructure.Persistence.Repositories;

public class NotificationRepository
    : TenantBaseRepository<Notification>, INotificationRepository
{
    public NotificationRepository(
        TenantDbContext context,
        ITenantContext tenantContext)
        : base(context, tenantContext)
    {
    }
}