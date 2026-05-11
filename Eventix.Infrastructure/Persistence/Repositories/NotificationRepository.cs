using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Persistence.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly TenantDbContext _context;

    public NotificationRepository(TenantDbContext context)
    {
        _context = context;
    }

    public Task<List<Notification>> GetAllAsync(Guid tenantId, CancellationToken ct)
        => _context.Notifications
            .Where(x => x.TenantId == tenantId && !x.IsDeleted)
            .ToListAsync(ct);

    public Task<Notification?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken ct)
        => _context.Notifications
            .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId, ct);

    public Task AddAsync(Notification entity, CancellationToken ct)
        => _context.Notifications.AddAsync(entity, ct).AsTask();

    public Task SaveChangesAsync(CancellationToken ct)
        => _context.SaveChangesAsync(ct);
}