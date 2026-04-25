using Eventix.Application.DTOs.EventCategories;
using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;

namespace Eventix.Application.Services;

public class EventCategoryService : IEventCategoryService
{
    private readonly IEventCategoryRepository _repository;
    private readonly ITenantContext _tenantContext;

    public EventCategoryService(
        IEventCategoryRepository repository,
        ITenantContext tenantContext)
    {
        _repository = repository;
        _tenantContext = tenantContext;
    }

    public async Task<List<EventCategoryResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetAllAsync(cancellationToken);

        return entities
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .Select(MapToResponseDto)
            .ToList();
    }

    public async Task<EventCategoryResponseDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);

        if (entity is null)
            return null;

        return MapToResponseDto(entity);
    }

    public async Task<EventCategoryResponseDTO> CreateAsync(
        CreateEventCategoryDTO dto,
        CancellationToken cancellationToken = default)
    {
        var entity = new EventCategory
        {
            Id = Guid.NewGuid(),
            TenantId = _tenantContext.TenantId,

            Name = dto.Name,
            Description = dto.Description,
            Icon = dto.Icon,
            DisplayOrder = dto.DisplayOrder,
            IsActive = dto.IsActive,

            CreatedAtUtc = DateTime.UtcNow
        };

        await _repository.AddAsync(entity, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return MapToResponseDto(entity);
    }

    public async Task<bool> UpdateAsync(
        Guid id,
        UpdateEventCategoryDTO dto,
        CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);

        if (entity is null)
            return false;

        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.Icon = dto.Icon;
        entity.DisplayOrder = dto.DisplayOrder;
        entity.IsActive = dto.IsActive;
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

    private static EventCategoryResponseDTO MapToResponseDto(EventCategory entity)
    {
        return new EventCategoryResponseDTO
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            Name = entity.Name,
            Description = entity.Description,
            Icon = entity.Icon,
            DisplayOrder = entity.DisplayOrder,
            IsActive = entity.IsActive,
            CreatedAtUtc = entity.CreatedAtUtc
        };
    }
}