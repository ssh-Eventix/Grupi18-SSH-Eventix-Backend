using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Eventix.Infrastructure.MultiTenancy;
using Eventix.Application.Interfaces.Common;

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

    public async Task<List<VenueSection>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.VenueSections
            .AsNoTracking()
            .Where(x => x.TenantId == _tenantContext.TenantId && !x.IsDeleted)
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync(cancellationToken);

    public async Task<List<VenueSection>> GetByVenueIdAsync(Guid venueId, CancellationToken cancellationToken = default)
        => await _context.VenueSections
            .AsNoTracking()
            .Where(x => x.TenantId == _tenantContext.TenantId &&
                        x.VenueId == venueId &&
                        !x.IsDeleted)
            .ToListAsync(cancellationToken);

    public async Task<VenueSection?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.VenueSections
            .FirstOrDefaultAsync(x => x.Id == id &&
                                     x.TenantId == _tenantContext.TenantId &&
                                     !x.IsDeleted, cancellationToken);

    public async Task AddAsync(VenueSection entity, CancellationToken cancellationToken = default)
        => await _context.VenueSections.AddAsync(entity, cancellationToken);

    public Task UpdateAsync(VenueSection entity)
    {
        _context.VenueSections.Update(entity);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}