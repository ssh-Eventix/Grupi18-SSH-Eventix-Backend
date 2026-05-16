using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Persistence.Repositories;

public class VenueRepository : TenantBaseRepository<Venue>, IVenueRepository
{
    public VenueRepository(
        TenantDbContext context,
        ITenantContext tenantContext)
        : base(context, tenantContext)
    {
    }

    public override Task<List<Venue>> GetAllAsync(CancellationToken ct = default)
    {
        return Query()
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(ct);
    }

    public override Task<Venue?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return Query()
            .Include(x => x.Sections)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public Task<bool> ExistsByCodeAsync(
        string code,
        Guid? excludeId = null,
        CancellationToken ct = default)
    {
        return Query().AnyAsync(x =>
            x.Code == code &&
            (excludeId == null || x.Id != excludeId),
            ct);
    }
}