using Eventix.Application.Interfaces.Common;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using System.Linq;

public class EventSessionRepository : IEventSessionRepository
{
    private readonly TenantDbContext _context;
    private readonly ITenantContext _tenantContext;

    public EventSessionRepository(TenantDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public Task<List<EventSession>> GetAllAsync(CancellationToken cancellationToken = default)
        => _context.EventSessions
            .AsNoTracking()
            .Where(x => x.TenantId == _tenantContext.TenantId && !x.IsDeleted)
            .ToListAsync(cancellationToken);

    public Task<EventSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.EventSessions
            .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == _tenantContext.TenantId && !x.IsDeleted, cancellationToken);

    public Task<List<EventSession>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default)
        => _context.EventSessions
            .AsNoTracking()
            .Where(x => x.EventId == eventId && x.TenantId == _tenantContext.TenantId && !x.IsDeleted)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(EventSession entity, CancellationToken cancellationToken = default)
        => await _context.EventSessions.AddAsync(entity, cancellationToken);

    public Task UpdateAsync(EventSession entity)
    {
        _context.EventSessions.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(EventSession entity)
    {
        _context.EventSessions.Remove(entity);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}