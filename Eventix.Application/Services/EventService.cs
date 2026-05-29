using System.Text.Json;
using Eventix.Application.DTOs.AuditLog;
using Eventix.Application.DTOs.Events;
using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;
using Eventix.Domain.Enums;

namespace Eventix.Application.Services;

public class EventService : IEventService
{
    private readonly IEventRepository _repository;
    private readonly ITenantContext _tenantContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuditLogService _auditLogService;

    public EventService(
    IEventRepository repository,
    ITenantContext tenantContext,
    ICurrentUserService currentUserService,
    IAuditLogService auditLogService)
    {
        _repository = repository;
        _tenantContext = tenantContext;
        _currentUserService = currentUserService;
        _auditLogService = auditLogService;
    }

    public async Task<List<EventResponseDTO>> GetAllAsync(string? search, CancellationToken cancellationToken = default)
    {
        var events = await _repository.GetAllAsync(search, cancellationToken);
        return events.Select(MapToResponseDto).ToList();
    }

    public async Task<EventResponseDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : MapToResponseDto(entity);
    }

    public async Task<EventResponseDTO> CreateAsync(CreateEventDTO dto, CancellationToken cancellationToken = default)
    {
        var entity = new Event
        {
            Id = Guid.NewGuid(),
            TenantId = _tenantContext.TenantId,

            VenueId = dto.VenueId,
            EventCategoryId = dto.EventCategoryId,

            Title = dto.Title,
            Slug = dto.Slug,
            Description = dto.Description,

            OrganizerName = dto.OrganizerName,

            StartUtc = dto.StartUtc,
            EndUtc = dto.EndUtc,

            Status = dto.Status,
            Visibility = (EventVisibility)dto.Visibility,

            BannerImageUrl = dto.BannerImageUrl,

            MaxTicketsPerOrder = dto.MaxTicketsPerOrder,
            MinTicketsPerOrder = dto.MinTicketsPerOrder,

            IsFree = dto.IsFree,
            IsPublished = dto.Status == EventStatus.Published || dto.IsPublished,

            Currency = dto.Currency,

            CreatedAtUtc = DateTime.UtcNow
        };

        await _repository.AddAsync(entity, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogService.CreateAsync(new CreateAuditLogDTO
        {
            TenantId = _tenantContext.TenantId,
            TenantName = _tenantContext.SchemaName,
            UserId = _currentUserService.UserId,
            UserEmail = _currentUserService.Email,
            EntityName = nameof(Event),
            EntityId = entity.Id,
            Action = AuditAction.Create,
            NewValues = JsonSerializer.Serialize(new
            {
                entity.Id,
                entity.Title,
                entity.Slug,
                entity.StartUtc,
                entity.EndUtc,
                entity.Status,
                entity.Visibility,
                entity.IsFree,
                entity.IsPublished,
                entity.Currency
            })
        }, cancellationToken);

        return MapToResponseDto(entity);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateEventDTO dto, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);

        if (entity is null)
            return false;

        var oldValues = JsonSerializer.Serialize(new
        {
            entity.Id,
            entity.Title,
            entity.Slug,
            entity.StartUtc,
            entity.EndUtc,
            entity.Status,
            entity.Visibility,
            entity.IsFree,
            entity.IsPublished,
            entity.Currency
        });

        entity.VenueId = dto.VenueId;
        entity.EventCategoryId = dto.EventCategoryId;

        entity.Title = dto.Title;
        entity.Slug = dto.Slug;
        entity.Description = dto.Description;

        entity.OrganizerName = dto.OrganizerName;

        entity.StartUtc = dto.StartUtc;
        entity.EndUtc = dto.EndUtc;

        entity.Status = (EventStatus)dto.Status;
        entity.Visibility = (EventVisibility)dto.Visibility;

        entity.BannerImageUrl = dto.BannerImageUrl;

        entity.MaxTicketsPerOrder = dto.MaxTicketsPerOrder;
        entity.MinTicketsPerOrder = dto.MinTicketsPerOrder;

        entity.IsFree = dto.IsFree;
        entity.IsPublished = dto.IsPublished;

        entity.Currency = dto.Currency;

        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _repository.UpdateAsync(entity);
        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogService.CreateAsync(new CreateAuditLogDTO
        {
            TenantId = _tenantContext.TenantId,
            TenantName = _tenantContext.SchemaName,
            UserId = _currentUserService.UserId,
            UserEmail = _currentUserService.Email,
            EntityName = nameof(Event),
            EntityId = entity.Id,
            Action = AuditAction.Update,
            OldValues = oldValues,
            NewValues = JsonSerializer.Serialize(new
            {
                entity.Id,
                entity.Title,
                entity.Slug,
                entity.StartUtc,
                entity.EndUtc,
                entity.Status,
                entity.Visibility,
                entity.IsFree,
                entity.IsPublished,
                entity.Currency
            })
        }, cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);

        if (entity is null)
            return false;

        var oldValues = JsonSerializer.Serialize(new
        {
            entity.Id,
            entity.Title,
            entity.Slug,
            entity.Status,
            entity.IsPublished
        });

        entity.IsDeleted = true;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _repository.UpdateAsync(entity);
        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogService.CreateAsync(new CreateAuditLogDTO
        {
            TenantId = _tenantContext.TenantId,
            TenantName = _tenantContext.SchemaName,
            UserId = _currentUserService.UserId,
            UserEmail = _currentUserService.Email,
            EntityName = nameof(Event),
            EntityId = entity.Id,
            Action = AuditAction.Delete,
            OldValues = oldValues
        }, cancellationToken);

        return true;
    }

    private static EventResponseDTO MapToResponseDto(Event entity)
    {
        return new EventResponseDTO
        {
            Id = entity.Id,

            VenueId = entity.VenueId,
            VenueName = entity.Venue?.Name,

            EventCategoryId = entity.EventCategoryId,
            EventCategoryName = entity.EventCategory?.Name,

            Title = entity.Title,
            Slug = entity.Slug,
            Description = entity.Description,

            OrganizerName = entity.OrganizerName,

            StartUtc = entity.StartUtc,
            EndUtc = entity.EndUtc,

            Status = entity.Status,
            Visibility = entity.Visibility,

            BannerImageUrl = entity.BannerImageUrl,

            MaxTicketsPerOrder = entity.MaxTicketsPerOrder,
            MinTicketsPerOrder = entity.MinTicketsPerOrder,

            IsFree = entity.IsFree,
            IsPublished = entity.IsPublished,

            Currency = entity.Currency,

            CreatedAtUtc = entity.CreatedAtUtc,

            SpeakerName = entity.Sessions
            .Where(x => x.Speaker != null)
            .OrderBy(x => x.StartTime)
            .Select(x => x.Speaker!.FullName)
            .FirstOrDefault(),
        };
    }
}