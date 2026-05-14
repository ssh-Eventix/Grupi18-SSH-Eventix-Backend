using Eventix.Application.DTOs.Notifications;
using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;

namespace Eventix.Application.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _repo;
    private readonly ITenantContext _tenant;

    public NotificationService(INotificationRepository repo, ITenantContext tenant)
    {
        _repo = repo;
        _tenant = tenant;
    }

    public async Task<List<NotificationDto>> GetAllAsync(CancellationToken ct)
    {
        var data = await _repo.GetAllAsync(_tenant.TenantId, ct);
        return data.Select(Map).ToList();
    }

    public async Task<NotificationDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var data = await _repo.GetByIdAsync(id, _tenant.TenantId, ct);
        return data is null ? null : Map(data);
    }

    public async Task<NotificationDto> CreateAsync(CreateNotificationDTO dto, CancellationToken ct)
    {
        var entity = new Notification
        {
            Id = Guid.NewGuid(),
            TenantId = _tenant.TenantId,
            UserId = dto.UserId,
            EventId = dto.EventId,
            Type = (Domain.Enums.NotificationType)dto.Type,
            Title = dto.Title,
            Message = dto.Message,
            SentAt = DateTime.UtcNow
        };

        await _repo.AddAsync(entity, ct);
        await _repo.SaveChangesAsync(ct);

        return Map(entity);
    }

    private static NotificationDto Map(Notification x) => new()
    {
        Id = x.Id,
        UserId = x.UserId,
        EventId = x.EventId,
        Type = (int)x.Type,
        Title = x.Title,
        Message = x.Message,
        IsRead = x.IsRead,
        SentAt = x.SentAt
    };
}