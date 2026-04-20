using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Persistence.Repositories;

public class VenueRepository : IVenueRepository
{
    private readonly TenantDbContext _context;

    public VenueRepository(TenantDbContext context)
    {
        _context = context;
    }

    public Task<List<Venue>> GetAllAsync(CancellationToken cancellationToken = default)
        => _context.Venues.AsNoTracking().ToListAsync(cancellationToken);

    public Task<Venue?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.Venues.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task AddAsync(Venue entity, CancellationToken cancellationToken = default)
        => await _context.Venues.AddAsync(entity, cancellationToken);

    public Task UpdateAsync(Venue entity)
    {
        _context.Venues.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Venue entity)
    {
        _context.Venues.Remove(entity);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}