using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Persistence.Repositories;

public class VenueRepository : IVenueRepository
{
    private readonly TenantDbContext _context;
    private readonly ITenantContext _tenantContext;

    public VenueRepository(TenantDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<List<Venue>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Venues
            .AsNoTracking()
            .Where(x => x.TenantId == _tenantContext.TenantId && !x.IsDeleted)
            .ToListAsync(cancellationToken);

    public async Task<Venue?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Venues
            .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == _tenantContext.TenantId && !x.IsDeleted, cancellationToken);

    public async Task AddAsync(Venue entity, CancellationToken cancellationToken = default)
        => await _context.Venues.AddAsync(entity, cancellationToken);

    public Task UpdateAsync(Venue entity)
    {
        _context.Venues.Update(entity);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}