using Eventix.Application.DTOs.Roles;

namespace Eventix.Application.Interfaces.Services;

public interface IRoleService
{
    Task<List<RoleResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<RoleResponseDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<RoleResponseDTO> CreateAsync(CreateRoleDTO dto, Guid tenantId, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Guid id, UpdateRoleDTO dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

