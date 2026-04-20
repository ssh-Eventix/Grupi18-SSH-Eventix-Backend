using Eventix.Domain.Entities;

namespace Eventix.Application.Interfaces.Repositories;

public interface IEventCategoryRepository
{
    Task<List<EventCategory>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<EventCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(EventCategory entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(EventCategory entity);
    Task DeleteAsync(EventCategory entity);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}