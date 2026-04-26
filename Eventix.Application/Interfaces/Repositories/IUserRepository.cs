using Eventix.Domain.Entities;

namespace Eventix.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task AddAsync(User entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(User entity);
    Task DeleteAsync(User entity);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}