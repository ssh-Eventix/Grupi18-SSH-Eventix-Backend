using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Persistence.Repositories;

public class CheckInRepository : TenantBaseRepository<CheckIn>, ICheckInRepository
{
    public CheckInRepository(
        TenantDbContext context,
        ITenantContext tenantContext)
        : base(context, tenantContext)
    {
    }

    public Task<CheckIn?> GetByTicketIdAsync(Guid ticketId, CancellationToken ct = default)
    {
        return Query()
            .FirstOrDefaultAsync(x => x.TicketId == ticketId, ct);
    }
}