using Eventix.Domain.Entities;

namespace Eventix.Application.Interfaces.Repositories;

using System;


public interface IReviewRepository
{
    Task<List<Review>> GetAllAsync(CancellationToken ct = default);
    Task<Review?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task AddAsync(Review entity, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}