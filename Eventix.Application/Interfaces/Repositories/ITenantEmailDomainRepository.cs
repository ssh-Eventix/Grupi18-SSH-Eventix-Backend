using Eventix.Domain.Entities;

namespace Eventix.Application.Interfaces.Repositories;

public interface ITenantEmailDomainRepository
{
    Task<List<TenantEmailDomain>> GetAllAsync(CancellationToken cancellationToken);
    Task<List<TenantEmailDomain>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken);
    Task<TenantEmailDomain?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<TenantEmailDomain?> GetByDomainAsync(string domain, CancellationToken cancellationToken);
    Task AddAsync(TenantEmailDomain entity, CancellationToken cancellationToken);
    Task UpdateAsync(TenantEmailDomain entity, CancellationToken cancellationToken);
    Task DeleteAsync(TenantEmailDomain entity, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);

    Task<TenantEmailDomain?> GetByTenantIdAndDomainAsync(
        Guid tenantId,
        string domain,
        CancellationToken cancellationToken);
}