using Eventix.Domain.Entities;

namespace Eventix.Application.Interfaces.Repositories;

public interface IEventRepository
{
    Task<List<Event>> GetAllAsync(string? search = null, CancellationToken cancellationToken = default);
    Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Event entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Event entity);
    Task DeleteAsync(Event entity);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}