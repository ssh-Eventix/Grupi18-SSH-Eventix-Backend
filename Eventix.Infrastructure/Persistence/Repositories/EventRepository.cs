using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.MultiTenancy;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Persistence.Repositories;

public class EventRepository : IEventRepository
{
    private readonly TenantDbContext _context;
    private readonly ITenantContext _tenantContext;

    public EventRepository(TenantDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<List<Event>> GetAllAsync(string? search = null, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantContext.TenantId;

        IQueryable<Event> query = _context.Events
            .AsNoTracking()
            .Include(x => x.Venue)
            .Include(x => x.EventCategory)
            .Where(x => x.TenantId == tenantId && !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim().ToLower();

            query = query.Where(x =>
                x.Title.ToLower().Contains(normalizedSearch) ||
                (x.Subtitle != null && x.Subtitle.ToLower().Contains(normalizedSearch)) ||
                (x.EventCategory != null && x.EventCategory.Name.ToLower().Contains(normalizedSearch)) ||
                (x.Venue != null && x.Venue.Name.ToLower().Contains(normalizedSearch)));
        }

        return await query
            .OrderByDescending(x => x.StartUtc)
            .ToListAsync(cancellationToken);
    }

    public Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantContext.TenantId;

        return _context.Events
            .Include(x => x.Venue)
            .Include(x => x.EventCategory)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.TenantId == tenantId &&
                !x.IsDeleted, cancellationToken);
    }

    public async Task AddAsync(Event entity, CancellationToken cancellationToken = default)
        => await _context.Events.AddAsync(entity, cancellationToken);

    public Task UpdateAsync(Event entity)
    {
        _context.Events.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Event entity)
    {
        _context.Events.Remove(entity);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}