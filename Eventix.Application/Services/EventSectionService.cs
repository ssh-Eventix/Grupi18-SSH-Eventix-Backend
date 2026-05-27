using Eventix.Application.DTOs.EventSections;
using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;

namespace Eventix.Application.Services;

public class EventSectionService : IEventSectionService
{
    private readonly IEventSectionRepository _repository;
    private readonly IEventRepository _eventRepository;
    private readonly IVenueSectionRepository _venueSectionRepository;
    private readonly ITenantContext _tenantContext;

    public EventSectionService(
       IEventSectionRepository repository,
       IEventRepository eventRepository,
       IVenueSectionRepository venueSectionRepository,
       ITenantContext tenantContext)
    {
        _repository = repository;
        _eventRepository = eventRepository;
        _venueSectionRepository = venueSectionRepository;
        _tenantContext = tenantContext;
    }

    public async Task<IEnumerable<EventSectionResponseDTO>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return entities.Select(Map);
    }

    public async Task<EventSectionResponseDTO?> GetByIdAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity is null ? null : Map(entity);
    }

    public async Task<IEnumerable<EventSectionResponseDTO>> GetByEventIdAsync(Guid eventId)
    {
        var entities = await _repository.GetByEventIdAsync(eventId);
        return entities.Select(Map);
    }

    public async Task<EventSectionResponseDTO> CreateAsync(CreateEventSectionDTO dto)
    {
        if (dto.Capacity <= 0)
            throw new InvalidOperationException("Event section capacity must be greater than zero.");

        var eventEntity = await _eventRepository.GetByIdAsync(dto.EventId);

        if (eventEntity == null)
            throw new InvalidOperationException("Event not found.");

        var venueSection = await _venueSectionRepository.GetByIdAsync(dto.VenueSectionId);

        if (venueSection == null)
            throw new InvalidOperationException("Venue section not found.");

        if (venueSection.VenueId != eventEntity.VenueId)
            throw new InvalidOperationException("Selected venue section does not belong to the event venue.");

        if (dto.Capacity > venueSection.Capacity)
            throw new InvalidOperationException(
                $"Event section capacity cannot exceed venue section capacity of {venueSection.Capacity}.");

        var exists = await _repository.ExistsByEventAndVenueSectionAsync(
            dto.EventId,
            dto.VenueSectionId);

        if (exists)
            throw new Exception("Event section already exists for this event and venue section.");

        var entity = new EventSection
        {
            Id = Guid.NewGuid(),
            TenantId = _tenantContext.TenantId,

            EventId = dto.EventId,
            VenueSectionId = dto.VenueSectionId,

            Name = dto.Name,
            Code = dto.Code,
            Capacity = dto.Capacity,
            IsActive = dto.IsActive,

            SalesStartUtc = dto.SalesStartUtc,
            SalesEndUtc = dto.SalesEndUtc,

            CreatedAtUtc = DateTime.UtcNow,
            IsDeleted = false
        };

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        return Map(entity);
    }

    public async Task<EventSectionResponseDTO?> UpdateAsync(Guid id, UpdateEventSectionDTO dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is null) return null;

        if (dto.Capacity <= 0)
            throw new InvalidOperationException("Event section capacity must be greater than zero.");

        var venueSection = await _venueSectionRepository.GetByIdAsync(entity.VenueSectionId);

        if (venueSection == null)
            throw new InvalidOperationException("Venue section not found.");

        if (dto.Capacity > venueSection.Capacity)
            throw new InvalidOperationException(
                $"Event section capacity cannot exceed venue section capacity of {venueSection.Capacity}.");

        entity.Name = dto.Name;
        entity.Code = dto.Code;
        entity.Capacity = dto.Capacity;
        entity.IsActive = dto.IsActive;
        entity.SalesStartUtc = dto.SalesStartUtc;
        entity.SalesEndUtc = dto.SalesEndUtc;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _repository.UpdateAsync(entity);
        await _repository.SaveChangesAsync();

        return Map(entity);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is null) return false;

        await _repository.DeleteAsync(entity);
        await _repository.SaveChangesAsync();

        return true;
    }

    private static EventSectionResponseDTO Map(EventSection x) => new()
    {
        Id = x.Id,

        EventId = x.EventId,
        EventTitle = x.Event?.Title,

        VenueSectionId = x.VenueSectionId,
        VenueSectionName = x.VenueSection?.Name,

        Name = x.Name,
        Code = x.Code,
        Capacity = x.Capacity,
        IsActive = x.IsActive,

        SalesStartUtc = x.SalesStartUtc,
        SalesEndUtc = x.SalesEndUtc,
        CreatedAtUtc = x.CreatedAtUtc
    };
}