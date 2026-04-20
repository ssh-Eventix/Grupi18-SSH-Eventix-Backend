using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Persistence.Repositories;

public class EventRepository : IEventRepository
{
    private readonly TenantDbContext _context;

    public EventRepository(TenantDbContext context)
    {
        _context = context;
    }

    public async Task<List<Event>> GetAllAsync(string? search = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Event> query = _context.Events
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.Venue);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.Title.ToLower().Contains(search.ToLower()));
        }

        return await query.OrderByDescending(x => x.StartDateUtc).ToListAsync(cancellationToken);
    }

    public Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.Events
            .Include(x => x.Category)
            .Include(x => x.Venue)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

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