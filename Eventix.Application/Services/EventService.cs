using Eventix.Application.DTOs.Events;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;
using Eventix.Domain.Enums;
using Eventix.Application.Interfaces.Common;

namespace Eventix.Application.Services;

public class EventService : IEventService
{
    private readonly IEventRepository _repository;
    private readonly ITenantContext _tenantContext;

    public EventService(IEventRepository repository, ITenantContext tenantContext)
    {
        _repository = repository;
        _tenantContext = tenantContext;
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
            TenantId = _tenantContext.TenantId!.Value,

            VenueId = dto.VenueId,
            EventCategoryId = dto.EventCategoryId,

            Title = dto.Title,
            Slug = dto.Slug,
            Description = dto.Description,

            OrganizerName = dto.OrganizerName,

            StartUtc = dto.StartUtc,
            EndUtc = dto.EndUtc,

            Status = EventStatus.Draft,
            Visibility = (EventVisibility)dto.Visibility,

            BannerImageUrl = dto.BannerImageUrl,

            MaxTicketsPerOrder = dto.MaxTicketsPerOrder,
            MinTicketsPerOrder = dto.MinTicketsPerOrder,

            IsFree = dto.IsFree,
            IsPublished = false,

            Currency = dto.Currency,

            CreatedAtUtc = DateTime.UtcNow
        };

        await _repository.AddAsync(entity, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return MapToResponseDto(entity);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateEventDTO dto, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);

        if (entity is null)
            return false;

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

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);

        if (entity is null)
            return false;

        entity.IsDeleted = true;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _repository.UpdateAsync(entity);
        await _repository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static EventResponseDTO MapToResponseDto(Event entity)
    {
        return new EventResponseDTO
        {
            Id = entity.Id,
            TenantId = entity.TenantId,

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

            Status = entity.Status.ToString(),
            Visibility = entity.Visibility.ToString(),

            BannerImageUrl = entity.BannerImageUrl,

            MaxTicketsPerOrder = entity.MaxTicketsPerOrder,
            MinTicketsPerOrder = entity.MinTicketsPerOrder,

            IsFree = entity.IsFree,
            IsPublished = entity.IsPublished,

            Currency = entity.Currency,

            CreatedAtUtc = entity.CreatedAtUtc
        };
    }
}