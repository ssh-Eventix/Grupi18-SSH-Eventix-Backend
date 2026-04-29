using Eventix.Application.DTOs.UserRoles;

namespace Eventix.Application.Interfaces.Services;

public interface IUserRoleService
{
    Task<List<UserRoleResponseDTO>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserRoleResponseDTO> AssignAsync(CreateUserRoleDTO dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

