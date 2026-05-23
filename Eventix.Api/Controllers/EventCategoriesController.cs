using Eventix.Api.Helpers;
using Eventix.Application.DTOs.EventCategories;
using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace Eventix.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventCategoriesController : ControllerBase
{
    private readonly IEventCategoryService _eventCategoryService;
    private readonly IDistributedCache _cache;
    private readonly ITenantContext _tenantContext;

    public EventCategoriesController(
    IEventCategoryService eventCategoryService,
    IDistributedCache cache,
    ITenantContext tenantContext)
    {
        _eventCategoryService = eventCategoryService;
        _cache = cache;
        _tenantContext = tenantContext;
    }

    [HttpGet]
    [Authorize(Policy = "Permission:ViewEventCategories")]
    public async Task<ActionResult<IEnumerable<EventCategoryResponseDTO>>> GetAll(
    CancellationToken cancellationToken)
    {
        var cacheKey = $"tenant:{_tenantContext.TenantId}:eventcategories:all";

        var result = await CacheHelper.GetOrSetAsync(
            _cache,
            cacheKey,
            () => _eventCategoryService.GetAllAsync(cancellationToken),
            TimeSpan.FromMinutes(10),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Permission:ViewEventCategories")]
    public async Task<ActionResult<EventCategoryResponseDTO>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _eventCategoryService.GetByIdAsync(id, cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "Permission:CreateEventCategories")]
    public async Task<ActionResult<EventCategoryResponseDTO>> Create(
        [FromBody] CreateEventCategoryDTO dto,
        CancellationToken cancellationToken)
    {
        var result = await _eventCategoryService.CreateAsync(dto, cancellationToken);

        await _cache.RemoveAsync(
            $"tenant:{_tenantContext.TenantId}:eventcategories:all",
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Permission:UpdateEventCategories")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateEventCategoryDTO dto,
        CancellationToken cancellationToken)
    {
        var updated = await _eventCategoryService.UpdateAsync(id, dto, cancellationToken);

        if (!updated)
            return NotFound();

        await _cache.RemoveAsync(
            $"tenant:{_tenantContext.TenantId}:eventcategories:all",
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Permission:DeleteEventCategories")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var deleted = await _eventCategoryService.DeleteAsync(id, cancellationToken);

        if (!deleted)
            return NotFound();

        await _cache.RemoveAsync(
            $"tenant:{_tenantContext.TenantId}:eventcategories:all",
            cancellationToken);

        return NoContent();
    }
}