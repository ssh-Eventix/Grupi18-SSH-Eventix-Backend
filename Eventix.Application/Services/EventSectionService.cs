using Eventix.Application.DTOs.EventSections;
using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;
using Eventix.Application.Interfaces.Common;

namespace Eventix.Application.Services;

public class EventSectionService : IEventSectionService
{
    private readonly IEventSectionRepository _repository;
    private readonly ITenantContext _tenantContext;

    public EventSectionService(
        IEventSectionRepository repository,
        ITenantContext tenantContext)
    {
        _repository = repository;
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

    public async Task<EventSectionResponseDTO> CreateAsync(CreateEventSectionDTO dto)
    {
        var exists = await _repository.ExistsByEventAndVenueSectionAsync(
            dto.EventId,
            dto.VenueSectionId);

        if (exists)
            throw new Exception("EventSection already exists for this Event + VenueSection");

        var entity = new EventSection
        {
            TenantId = _tenantContext.TenantId, 
            EventId = dto.EventId,
            VenueSectionId = dto.VenueSectionId,
            Name = dto.Name,
            Code = dto.Code,
            Capacity = dto.Capacity,
            Price = dto.Price,
            SalesStartUtc = dto.SalesStartUtc,
            SalesEndUtc = dto.SalesEndUtc
        };

        await _repository.AddAsync(entity);

        return Map(entity);
    }

    public async Task<EventSectionResponseDTO?> UpdateAsync(Guid id, UpdateEventSectionDTO dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is null) return null;

        entity.Name = dto.Name;
        entity.Code = dto.Code;
        entity.Capacity = dto.Capacity;
        entity.Price = dto.Price;
        entity.IsActive = dto.IsActive;

        await _repository.UpdateAsync(entity);

        return Map(entity);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is null) return false;

        await _repository.DeleteAsync(entity);

        return true;
    }

    private static EventSectionResponseDTO Map(EventSection x) => new()
    {
        Id = x.Id,
        EventId = x.EventId,
        VenueSectionId = x.VenueSectionId,
        Name = x.Name,
        Code = x.Code,
        Capacity = x.Capacity,
        Price = x.Price,
        IsActive = x.IsActive
    };
}