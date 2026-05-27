using Eventix.Domain.Entities;

namespace Eventix.Application.Interfaces.Repositories;

public interface IReviewRepository
{
    Task<List<Review>> GetAllAsync(CancellationToken ct = default);
    Task<Review?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Review review, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);

    Task<List<Review>> GetByEventIdAsync(Guid eventId, CancellationToken ct = default);
    Task<List<Review>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
}