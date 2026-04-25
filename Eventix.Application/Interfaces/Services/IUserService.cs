using Eventix.Application.DTOs.Users;

namespace Eventix.Application.Interfaces.Services;

public interface IUserService
{
    Task<List<UserResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserResponseDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserResponseDTO?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<UserResponseDTO> CreateAsync(CreateUserDTO dto, Guid tenantId, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Guid id, UpdateUserDTO dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

