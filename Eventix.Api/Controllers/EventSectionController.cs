using Eventix.Api.Helpers;
using Eventix.Application.DTOs.EventSections;
using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace Eventix.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventSectionController : ControllerBase
{
    private readonly IEventSectionService _service;
    private readonly IDistributedCache _cache;
    private readonly ITenantContext _tenantContext;

    public EventSectionController(
         IEventSectionService service,
         IDistributedCache cache,
         ITenantContext tenantContext)
    {
        _service = service;
        _cache = cache;
        _tenantContext = tenantContext;
    }

    [HttpGet]
    [Authorize(Policy = "Permission:ViewEventSections")]
    public async Task<ActionResult<IEnumerable<EventSectionResponseDTO>>> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("event/{eventId:guid}")]
    [Authorize(Policy = "Permission:ViewEventSections")]
    public async Task<ActionResult<IEnumerable<EventSectionResponseDTO>>> GetByEventId(
         Guid eventId,
         CancellationToken cancellationToken)
    {
        var result = await _service.GetByEventIdAsync(eventId);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Permission:ViewEventSections")]
    public async Task<ActionResult<EventSectionResponseDTO>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"tenant:{_tenantContext.TenantId}:eventsection:{id}";

        var result = await CacheHelper.GetOrSetAsync(
            _cache,
            cacheKey,
            () => _service.GetByIdAsync(id),
            TimeSpan.FromMinutes(10),
            cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "Permission:ManageEventSections")]
    public async Task<ActionResult<EventSectionResponseDTO>> Create(
       [FromBody] CreateEventSectionDTO dto,
       CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(dto);

        await _cache.RemoveAsync(
            $"tenant:{_tenantContext.TenantId}:eventsections:all",
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Permission:ManageEventSections")]
    public async Task<ActionResult<EventSectionResponseDTO>> Update(
        Guid id,
        [FromBody] UpdateEventSectionDTO dto,
        CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(id, dto);

        if (result is null)
            return NotFound();

        await _cache.RemoveAsync(
            $"tenant:{_tenantContext.TenantId}:eventsections:all",
            cancellationToken);

        await _cache.RemoveAsync(
            $"tenant:{_tenantContext.TenantId}:eventsection:{id}",
            cancellationToken);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Permission:ManageEventSections")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var deleted = await _service.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        await _cache.RemoveAsync(
            $"tenant:{_tenantContext.TenantId}:eventsections:all",
            cancellationToken);

        await _cache.RemoveAsync(
            $"tenant:{_tenantContext.TenantId}:eventsection:{id}",
            cancellationToken);

        return NoContent();
    }

}