using Eventix.Domain.Entities;

namespace Eventix.Application.Interfaces.Repositories;

public interface INotificationRepository
{
    Task<List<Notification>> GetAllAsync(CancellationToken ct);
    Task<Notification?> GetByIdAsync(Guid id,CancellationToken ct);

    Task AddAsync(Notification entity, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}