using Eventix.Application.DTOs.TenantEmailDomains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventix.Application.Interfaces.Services
{
    public interface ITenantEmailDomainService
    {
        Task<List<TenantEmailDomainResponseDTO>> GetAllAsync(CancellationToken ct);
        Task<List<TenantEmailDomainResponseDTO>> GetByTenantIdAsync(Guid tenantId, CancellationToken ct);
        Task<TenantEmailDomainResponseDTO?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<TenantEmailDomainResponseDTO> CreateAsync(CreateTenantEmailDomainDTO dto, CancellationToken ct);
        Task<TenantEmailDomainResponseDTO?> UpdateAsync(Guid id, UpdateTenantEmailDomainDTO dto, CancellationToken ct);
        Task<bool> DeleteAsync(Guid id, CancellationToken ct);
    }
}
