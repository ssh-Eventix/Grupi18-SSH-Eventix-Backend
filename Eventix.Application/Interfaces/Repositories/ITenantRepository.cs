using Eventix.Domain.Entities;

namespace Eventix.Application.Interfaces.Repositories;

public interface ITenantRepository
{
    Task<List<Tenant>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Tenant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Tenant?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task AddAsync(Tenant tenant, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}