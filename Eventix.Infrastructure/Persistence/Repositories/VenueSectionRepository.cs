using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Persistence.Repositories;

public class VenueSectionRepository : IVenueSectionRepository
{
    private readonly TenantDbContext _context;
    private readonly ITenantContext _tenantContext;

    public VenueSectionRepository(TenantDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    private Guid TenantId => _tenantContext.TenantId;

    public async Task<List<VenueSection>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.VenueSections
            .AsNoTracking()
            .Include(x => x.Venue)
            .Where(x => x.TenantId == TenantId && !x.IsDeleted)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);

    public async Task<List<VenueSection>> GetByVenueIdAsync(Guid venueId, CancellationToken cancellationToken = default)
        => await _context.VenueSections
            .AsNoTracking()
            .Where(x =>
                x.TenantId == TenantId &&
                x.VenueId == venueId &&
                !x.IsDeleted)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);

    public async Task<VenueSection?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.VenueSections
            .Include(x => x.Venue)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.TenantId == TenantId &&
                !x.IsDeleted,
                cancellationToken);

    public async Task AddAsync(VenueSection entity, CancellationToken cancellationToken = default)
    {
        entity.TenantId = TenantId;
        await _context.VenueSections.AddAsync(entity, cancellationToken);
    }

    public Task UpdateAsync(VenueSection entity)
    {
        _context.VenueSections.Update(entity);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}