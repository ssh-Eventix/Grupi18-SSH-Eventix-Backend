using Eventix.Application.Interfaces.Common;
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

    private Guid TenantId => _tenantContext.TenantId;

    public async Task<List<Venue>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Venues
            .AsNoTracking()
            .Where(x => x.TenantId == TenantId && !x.IsDeleted)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

    public async Task<Venue?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Venues
            .Include(x => x.Sections)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.TenantId == TenantId &&
                !x.IsDeleted,
                cancellationToken);

    public async Task<bool> ExistsByCodeAsync(
        string code,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
        => await _context.Venues.AnyAsync(x =>
            x.TenantId == TenantId &&
            x.Code == code &&
            !x.IsDeleted &&
            (excludeId == null || x.Id != excludeId),
            cancellationToken);

    public async Task AddAsync(Venue venue, CancellationToken cancellationToken = default)
    {
        venue.TenantId = TenantId;
        await _context.Venues.AddAsync(venue, cancellationToken);
    }

    public Task UpdateAsync(Venue venue)
    {
        _context.Venues.Update(venue);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}