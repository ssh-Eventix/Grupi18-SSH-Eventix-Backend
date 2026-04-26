using Eventix.Domain.Entities;

namespace Eventix.Application.Interfaces.Repositories;

public interface IRoleRepository
{
    Task<List<Role>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Role entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Role entity);
    Task DeleteAsync(Role entity);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}