using Eventix.Domain.Entities;

namespace Eventix.Application.Interfaces.Repositories;

public interface ITenantEmailDomainRepository
{
    Task<TenantEmailDomain?> GetByTenantIdAndDomainAsync(
        Guid tenantId,
        string domain,
        CancellationToken ct);
}