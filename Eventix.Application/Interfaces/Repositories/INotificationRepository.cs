using Eventix.Domain.Entities;

namespace Eventix.Application.Interfaces.Repositories;

public interface INotificationRepository
{
    Task<List<Notification>> GetAllAsync(Guid tenantId, CancellationToken ct);
    Task<Notification?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken ct);

    Task AddAsync(Notification entity, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}