using Eventix.Domain.Entities;

namespace Eventix.Application.Interfaces.Repositories;

public interface ITenantEmailDomainRepository
{
    Task<List<TenantEmailDomain>> GetAllAsync(CancellationToken ct);
    Task<List<TenantEmailDomain>> GetByTenantIdAsync(Guid tenantId, CancellationToken ct);
    Task<TenantEmailDomain?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<TenantEmailDomain?> GetByDomainAsync(string domain, CancellationToken ct);
    Task<TenantEmailDomain?> GetByTenantIdAndDomainAsync(Guid tenantId, string domain, CancellationToken ct);
    Task<TenantEmailDomain?> GetAnyByTenantIdAndDomainAsync(Guid tenantId, string domain, CancellationToken ct);
    Task AddAsync(TenantEmailDomain entity, CancellationToken ct);
    Task UpdateAsync(TenantEmailDomain entity, CancellationToken ct);
    Task DeleteAsync(TenantEmailDomain entity, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
