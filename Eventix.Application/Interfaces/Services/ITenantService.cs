using System;
using System.Threading.Tasks;
using Eventix.Application.DTOs.Tenants;

namespace Eventix.Application.Interfaces.Services;

public interface ITenantService
{
    Task<IReadOnlyList<TenantResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TenantResponseDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TenantResponseDTO?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<TenantResponseDTO> CreateAsync(CreateTenantDTO dto, CancellationToken cancellationToken = default);
    Task<TenantResponseDTO?> UpdateAsync(Guid id, UpdateTenantDTO dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}