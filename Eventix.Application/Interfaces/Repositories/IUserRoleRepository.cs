using Eventix.Domain.Entities;

namespace Eventix.Application.Interfaces.Repositories;

public interface IUserRoleRepository
{
    Task<UserRole?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<UserRole>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(UserRole entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(UserRole entity);
    Task<bool> ExistsAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}