using Eventix.Domain.Entities;

namespace Eventix.Application.Interfaces.Repositories;

public interface IEventSectionRepository
{
    Task<IReadOnlyList<EventSection>> GetAllAsync(CancellationToken ct = default);

    Task<IReadOnlyList<EventSection>> GetByEventIdAsync(Guid eventId, CancellationToken ct = default);

    Task<EventSection?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task AddAsync(EventSection entity, CancellationToken ct = default);

    Task UpdateAsync(EventSection entity);

    Task DeleteAsync(EventSection entity);

    Task<bool> ExistsByEventAndVenueSectionAsync(
        Guid eventId,
        Guid venueSectionId,
        CancellationToken ct = default);
}
