using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Persistence.Repositories;

public class TenantEmailDomainRepository : ITenantEmailDomainRepository
{
    private readonly PublicDbContext _context;

    public TenantEmailDomainRepository(PublicDbContext context)
    {
        _context = context;
    }

    public Task<TenantEmailDomain?> GetByTenantIdAndDomainAsync(
        Guid tenantId,
        string domain,
        CancellationToken ct)
    {
        return _context.TenantEmailDomains
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.TenantId == tenantId &&
                x.Domain == domain.ToLower(),
                ct);
    }
}