using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Persistence.Repositories;

public class ReviewRepository : TenantBaseRepository<Review>, IReviewRepository
{
    private readonly TenantDbContext _context;

    public ReviewRepository(
        TenantDbContext context,
        ITenantContext tenantContext)
        : base(context, tenantContext)
    {
        _context = context;
    }

    public async Task<List<Review>> GetByEventIdAsync(
        Guid eventId,
        CancellationToken ct = default)
    {
        return await _context.Reviews
            .AsNoTracking()
            .Where(r => r.EventId == eventId && !r.IsDeleted)
            .OrderByDescending(r => r.CreatedAtUtc)
            .ToListAsync(ct);
    }

    public async Task<List<Review>> GetByUserIdAsync(
        Guid userId,
        CancellationToken ct = default)
    {
        return await _context.Reviews
            .AsNoTracking()
            .Where(r => r.UserId == userId && !r.IsDeleted)
            .OrderByDescending(r => r.CreatedAtUtc)
            .ToListAsync(ct);
    }
}