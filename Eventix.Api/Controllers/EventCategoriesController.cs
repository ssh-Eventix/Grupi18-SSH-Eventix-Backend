using Eventix.Application.DTOs.EventCategories;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.MultiTenancy;
using Microsoft.AspNetCore.Mvc;

namespace Eventix.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventCategoriesController : ControllerBase
{
    private readonly IEventCategoryRepository _repository;
    private readonly ITenantContext _tenantContext;

    public EventCategoriesController(
        IEventCategoryRepository repository,
        ITenantContext tenantContext)
    {
        _repository = repository;
        _tenantContext = tenantContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EventCategoryResponseDTO>>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await _repository.GetAllAsync(cancellationToken);

        var response = entities
            .Where(x => x.TenantId == _tenantContext.TenantId && !x.IsDeleted)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .Select(MapToResponseDto)
            .ToList();

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EventCategoryResponseDTO>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);

        if (entity is null || entity.TenantId != _tenantContext.TenantId || entity.IsDeleted)
            return NotFound();

        return Ok(MapToResponseDto(entity));
    }

    [HttpPost]
    public async Task<ActionResult<EventCategoryResponseDTO>> Create(
        [FromBody] CreateEventCategoryDTO dto,
        CancellationToken cancellationToken)
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

        var response = MapToResponseDto(entity);

        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateEventCategoryDTO dto,
        CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);

        if (entity is null || entity.TenantId != _tenantContext.TenantId || entity.IsDeleted)
            return NotFound();

        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.Icon = dto.Icon;
        entity.DisplayOrder = dto.DisplayOrder;
        entity.IsActive = dto.IsActive;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _repository.UpdateAsync(entity);
        await _repository.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);

        if (entity is null || entity.TenantId != _tenantContext.TenantId || entity.IsDeleted)
            return NotFound();

        entity.IsDeleted = true;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _repository.UpdateAsync(entity);
        await _repository.SaveChangesAsync(cancellationToken);

        return NoContent();
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