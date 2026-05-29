using Eventix.Application.DTOs.Notifications;

namespace Eventix.Application.Interfaces.Services;

public interface INotificationService
{
    Task<List<NotificationDto>> GetAllAsync(CancellationToken ct);
    Task<NotificationDto?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<NotificationDto> CreateAsync(CreateNotificationDTO dto, CancellationToken ct);
    Task<List<NotificationDto>> GetByUserIdAsync(Guid userId, CancellationToken ct);
}