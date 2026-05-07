using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Persistence.Repositories;

public class CheckInRepository : ICheckInRepository
{
    private readonly TenantDbContext _context;
    private readonly ITenantContext _tenant;

    public CheckInRepository(TenantDbContext context, ITenantContext tenant)
    {
        _context = context;
        _tenant = tenant;
    }

    public Task<List<CheckIn>> GetAllAsync(Guid tenantId, CancellationToken ct)
        => _context.CheckIns
            .Where(x => x.TenantId == tenantId && !x.IsDeleted)
            .ToListAsync(ct);

    public Task<CheckIn?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken ct)
        => _context.CheckIns
            .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId, ct);

    public Task<CheckIn?> GetByTicketIdAsync(Guid ticketId, Guid tenantId, CancellationToken ct)
        => _context.CheckIns
            .FirstOrDefaultAsync(x => x.TicketId == ticketId && x.TenantId == tenantId, ct);

    public Task AddAsync(CheckIn entity, CancellationToken ct)
        => _context.CheckIns.AddAsync(entity, ct).AsTask();

    public Task SaveChangesAsync(CancellationToken ct)
        => _context.SaveChangesAsync(ct);
}