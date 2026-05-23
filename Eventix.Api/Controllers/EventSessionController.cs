using Eventix.Api.Helpers;
using Eventix.Application.DTOs.EventSessions;
using Eventix.Application.Interfaces.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

[ApiController]
[Route("api/[controller]")]
public class EventSessionController : ControllerBase
{
    private readonly IEventSessionService _service;
    private readonly ITenantContext _tenantContext;
    private readonly IDistributedCache _cache;

    public EventSessionController(
       IEventSessionService service,
       ITenantContext tenantContext,
       IDistributedCache cache)
    {
        _service = service;
        _tenantContext = tenantContext;
        _cache = cache;
    }

    [HttpGet]
    [Authorize(Policy = "Permission:ViewEventSessions")]
    public async Task<ActionResult<IEnumerable<EventSessionResponseDTO>>> GetAll(
       CancellationToken cancellationToken)
    {
        var cacheKey = $"tenant:{_tenantContext.TenantId}:eventsessions:all";

        var sessions = await CacheHelper.GetOrSetAsync(
            _cache,
            cacheKey,
            () => _service.GetAllAsync(cancellationToken),
            TimeSpan.FromMinutes(10),
            cancellationToken);

        return Ok(sessions);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Permission:ViewEventSessions")]
    public async Task<ActionResult<EventSessionResponseDTO>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"tenant:{_tenantContext.TenantId}:eventsession:{id}";

        var session = await CacheHelper.GetOrSetAsync(
            _cache,
            cacheKey,
            () => _service.GetByIdAsync(id, cancellationToken),
            TimeSpan.FromMinutes(10),
            cancellationToken);

        return session is null ? NotFound() : Ok(session);
    }

    [HttpPost]
    [Authorize(Policy = "Permission:ManageEventSessions")]
    public async Task<ActionResult<EventSessionResponseDTO>> Create(
         [FromBody] CreateEventSessionDTO dto,
         CancellationToken cancellationToken)
    {
        var response = await _service.CreateAsync(
            dto,
            _tenantContext.TenantId,
            cancellationToken);

        await _cache.RemoveAsync(
            $"tenant:{_tenantContext.TenantId}:eventsessions:all",
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Permission:ManageEventSessions")]
    public async Task<IActionResult> Update(
       Guid id,
       [FromBody] UpdateEventSessionDTO dto,
       CancellationToken cancellationToken)
    {
        var updated = await _service.UpdateAsync(id, dto, cancellationToken);

        if (!updated)
            return NotFound();

        await _cache.RemoveAsync(
            $"tenant:{_tenantContext.TenantId}:eventsessions:all",
            cancellationToken);

        await _cache.RemoveAsync(
            $"tenant:{_tenantContext.TenantId}:eventsession:{id}",
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Permission:ManageEventSessions")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var deleted = await _service.DeleteAsync(id, cancellationToken);

        if (!deleted)
            return NotFound();

        await _cache.RemoveAsync(
            $"tenant:{_tenantContext.TenantId}:eventsessions:all",
            cancellationToken);

        await _cache.RemoveAsync(
            $"tenant:{_tenantContext.TenantId}:eventsession:{id}",
            cancellationToken);

        return NoContent();
    }
}