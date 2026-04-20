using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Persistence.Repositories;

public class VenueSectionRepository : IVenueSectionRepository
{
    private readonly TenantDbContext _context;

    public VenueSectionRepository(TenantDbContext context)
    {
        _context = context;
    }

    public Task<List<VenueSection>> GetAllAsync(CancellationToken cancellationToken = default)
        => _context.VenueSections.AsNoTracking().ToListAsync(cancellationToken);

    public Task<List<VenueSection>> GetByVenueIdAsync(Guid venueId, CancellationToken cancellationToken = default)
        => _context.VenueSections.Where(x => x.VenueId == venueId).AsNoTracking().ToListAsync(cancellationToken);

    public Task<VenueSection?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.VenueSections.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task AddAsync(VenueSection entity, CancellationToken cancellationToken = default)
        => await _context.VenueSections.AddAsync(entity, cancellationToken);

    public Task UpdateAsync(VenueSection entity)
    {
        _context.VenueSections.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(VenueSection entity)
    {
        _context.VenueSections.Remove(entity);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}