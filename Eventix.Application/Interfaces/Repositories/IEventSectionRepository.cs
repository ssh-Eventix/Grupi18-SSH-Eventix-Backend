using Eventix.Domain.Entities;

namespace Eventix.Application.Interfaces.Repositories;

public interface IEventSectionRepository
{
    Task<IReadOnlyList<EventSection>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<EventSection?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EventSection>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default);

    Task<bool> ExistsByEventAndVenueSectionAsync(
        Guid eventId,
        Guid venueSectionId,
        CancellationToken cancellationToken = default);

    Task AddAsync(EventSection entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(EventSection entity);
    Task DeleteAsync(EventSection entity);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}