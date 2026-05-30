using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Persistence.Repositories;

public class VenueSectionRepository : TenantBaseRepository<VenueSection>, IVenueSectionRepository
{
    public VenueSectionRepository(
        TenantDbContext context,
        ITenantContext tenantContext)
        : base(context, tenantContext)
    {
    }

    public override Task<List<VenueSection>> GetAllAsync(CancellationToken ct = default)
    {
        return Query()
            .AsNoTracking()
            .Include(x => x.Venue)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToListAsync(ct);
    }

    public Task<List<VenueSection>> GetByVenueIdAsync(Guid venueId, CancellationToken ct = default)
    {
        return Query()
            .AsNoTracking()
            .Where(x => x.VenueId == venueId)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToListAsync(ct);
    }

    public override Task<VenueSection?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return Query()
            .Include(x => x.Venue)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public Task<VenueSection?> GetByVenueIdAndCodeAsync(
        Guid venueId,
        string code,
        CancellationToken ct = default)
    {
        var normalizedCode = code.Trim().ToLower();

        return Query()
            .Include(x => x.Venue)
            .FirstOrDefaultAsync(
                x => x.VenueId == venueId &&
                     x.Code.ToLower() == normalizedCode,
                ct);
    }
}
