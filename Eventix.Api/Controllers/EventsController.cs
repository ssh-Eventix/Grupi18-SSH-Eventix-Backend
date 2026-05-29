using Eventix.Api.Helpers;
using Eventix.Application.DTOs.Events;
using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Eventix.Infrastructure.Services;

namespace Eventix.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;
    private readonly IDistributedCache _cache;
    private readonly ITenantContext _tenantContext;
    private readonly IPublicEventService _publicEventService;

    public EventsController(
     IEventService eventService,
     IPublicEventService publicEventService,
     IDistributedCache cache,
     ITenantContext tenantContext)
    {
        _eventService = eventService;
        _publicEventService = publicEventService;
        _cache = cache;
        _tenantContext = tenantContext;
    }

    [HttpGet]
    [Authorize(Policy = "Permission:ViewEvents")]
    public async Task<ActionResult<IEnumerable<EventResponseDTO>>> GetAll(
        [FromQuery] string? search,
        CancellationToken cancellationToken)
    {
        var result = await _eventService.GetAllAsync(search, cancellationToken);

        return Ok(result);
    }

    [HttpGet("search")]
    [Authorize(Policy = "Permission:SearchEvents")]
    public async Task<ActionResult<IEnumerable<EventResponseDTO>>> Search(
        [FromQuery] string? search,
        CancellationToken cancellationToken)
    {
        var result = await _eventService.GetAllAsync(search, cancellationToken);

        return Ok(result);
    }

    [HttpGet("public")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<EventResponseDTO>>> PublicSearch(
     [FromQuery] string? search,
     CancellationToken cancellationToken)
    {
        var cacheKey = $"events:public:search:{search ?? "all"}";

        var result = await CacheHelper.GetOrSetAsync(
            _cache,
            cacheKey,
            () => _publicEventService.GetAllPublicAsync(search, cancellationToken),
            TimeSpan.FromMinutes(5),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("public/{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<EventResponseDTO>> PublicGetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"events:public:{id}";

        var result = await CacheHelper.GetOrSetAsync(
            _cache,
            cacheKey,
            () => _publicEventService.GetPublicByIdAsync(id, cancellationToken),
            TimeSpan.FromMinutes(5),
            cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }


    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Permission:ViewEvents")]
    public async Task<ActionResult<EventResponseDTO>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _eventService.GetByIdAsync(id, cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "Permission:CreateEvents")]
    public async Task<ActionResult<EventResponseDTO>> Create(
        [FromBody] CreateEventDTO dto,
        CancellationToken cancellationToken)
    {
        var result = await _eventService.CreateAsync(dto, cancellationToken);

        await _cache.RemoveAsync(
            $"tenant:{_tenantContext.TenantId}:events:search:all",
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Permission:UpdateEvents")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateEventDTO dto,
        CancellationToken cancellationToken)
    {
        var updated = await _eventService.UpdateAsync(id, dto, cancellationToken);

        if (!updated)
            return NotFound();

        await _cache.RemoveAsync(
            $"tenant:{_tenantContext.TenantId}:event:{id}",
            cancellationToken);

        await _cache.RemoveAsync(
            $"tenant:{_tenantContext.TenantId}:events:search:all",
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Permission:DeleteEvents")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var deleted = await _eventService.DeleteAsync(id, cancellationToken);

        if (!deleted)
            return NotFound();

        await _cache.RemoveAsync(
            $"tenant:{_tenantContext.TenantId}:event:{id}",
            cancellationToken);

        await _cache.RemoveAsync(
            $"tenant:{_tenantContext.TenantId}:events:search:all",
            cancellationToken);

        return NoContent();
    }
}