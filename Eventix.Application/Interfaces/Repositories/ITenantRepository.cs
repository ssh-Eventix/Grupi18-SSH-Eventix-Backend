using Eventix.Domain.Entities;

namespace Eventix.Application.Interfaces.Repositories;

public interface ITenantRepository
{
    Task<List<Tenant>> GetAllAsync(CancellationToken ct);
    Task<Tenant?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<Tenant?> GetBySlugAsync(string slug, CancellationToken ct);

    Task AddAsync(Tenant entity, CancellationToken ct);
    Task UpdateAsync(Tenant entity);
    Task DeleteAsync(Tenant entity);

    Task SaveChangesAsync(CancellationToken ct);
}