using Eventix.Domain.Entities;

public interface IEventSessionRepository
{
    Task<List<EventSession>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<EventSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<EventSession>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task AddAsync(EventSession entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(EventSession entity);
    Task DeleteAsync(EventSession entity);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}