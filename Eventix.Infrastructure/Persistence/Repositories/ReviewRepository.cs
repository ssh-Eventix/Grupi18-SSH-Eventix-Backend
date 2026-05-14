using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

public class ReviewRepository : IReviewRepository
{
    private readonly TenantDbContext _context;

    public ReviewRepository(TenantDbContext context)
    {
        _context = context;
    }

    public Task<List<Review>> GetAllAsync(Guid tenantId, CancellationToken ct)
        => _context.Reviews
            .Where(x => x.TenantId == tenantId && !x.IsDeleted)
            .ToListAsync(ct);

    public Task<Review?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken ct)
        => _context.Reviews
            .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId, ct);

    public Task AddAsync(Review entity, CancellationToken ct)
        => _context.Reviews.AddAsync(entity, ct).AsTask();

    public Task SaveChangesAsync(CancellationToken ct)
        => _context.SaveChangesAsync(ct);
}