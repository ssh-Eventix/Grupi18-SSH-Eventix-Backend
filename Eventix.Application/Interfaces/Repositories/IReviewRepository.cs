using Eventix.Domain.Entities;

namespace Eventix.Application.Interfaces.Repositories;

using System;


public interface IReviewRepository
{
    Task<List<Review>> GetAllAsync(Guid tenantId, CancellationToken ct);
    Task<Review?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken ct);

    Task AddAsync(Review entity, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}