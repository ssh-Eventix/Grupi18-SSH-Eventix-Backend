using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Persistence.Repositories;

public class NotificationRepository : TenantBaseRepository<Notification>, INotificationRepository
{
    private readonly TenantDbContext _context;

    public NotificationRepository(
        TenantDbContext context,
        ITenantContext tenantContext)
        : base(context, tenantContext)
    {
        _context = context;
    }

    public override async Task<List<Notification>> GetAllAsync(CancellationToken ct)
    {
        return await _context.Notifications
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .OrderByDescending(x => x.SentAt)
            .ToListAsync(ct);
    }

    public override async Task<Notification?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.Notifications
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
    }

    public async Task<List<Notification>> GetByUserIdAsync(Guid userId, CancellationToken ct)
    {
        return await _context.Notifications
            .AsNoTracking()
            .Where(x => x.UserId == userId && !x.IsDeleted)
            .OrderByDescending(x => x.SentAt)
            .ToListAsync(ct);
    }

    public override async Task AddAsync(Notification entity, CancellationToken ct)
    {
        await _context.Notifications.AddAsync(entity, ct);
    }

    public override Task SaveChangesAsync(CancellationToken ct)
    {
        return _context.SaveChangesAsync(ct);
    }
}