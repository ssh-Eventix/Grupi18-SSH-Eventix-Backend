using Eventix.Api.Helpers;
using Eventix.Application.DTOs.Speaker;
using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace Eventix.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpeakersController : ControllerBase
{
    private readonly ISpeakerService _speakerService;
    private readonly IDistributedCache _cache;
    private readonly ITenantContext _tenantContext;

    public SpeakersController(
        ISpeakerService speakerService,
        IDistributedCache cache,
        ITenantContext tenantContext)
    {
        _speakerService = speakerService;
        _cache = cache;
        _tenantContext = tenantContext;
    }

    [HttpGet]
    [Authorize(Policy = "Permission:ViewSpeakers")]
    public async Task<ActionResult<List<SpeakerDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var cacheKey = $"tenant:{_tenantContext.TenantId}:speakers:all";

        var speakers = await CacheHelper.GetOrSetAsync(
            _cache,
            cacheKey,
            () => _speakerService.GetAllAsync(cancellationToken),
            TimeSpan.FromMinutes(10),
            cancellationToken);

        return Ok(speakers);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Permission:ViewSpeakers")]
    public async Task<ActionResult<SpeakerDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"tenant:{_tenantContext.TenantId}:speaker:{id}";

        var speaker = await CacheHelper.GetOrSetAsync(
            _cache,
            cacheKey,
            () => _speakerService.GetByIdAsync(id, cancellationToken),
            TimeSpan.FromMinutes(10),
            cancellationToken);

        return speaker is null ? NotFound() : Ok(speaker);
    }

    [HttpPost]
    [Authorize(Policy = "Permission:CreateSpeakers")]
    public async Task<ActionResult<SpeakerDto>> Create(
       [FromBody] CreateSpeakerDto dto,
       CancellationToken cancellationToken)
    {
        var createdSpeaker = await _speakerService.CreateAsync(dto, cancellationToken);

        await _cache.RemoveAsync(
            $"tenant:{_tenantContext.TenantId}:speakers:all",
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = createdSpeaker.Id }, createdSpeaker);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Permission:UpdateSpeakers")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateSpeakerDto dto,
        CancellationToken cancellationToken)
    {
        var updated = await _speakerService.UpdateAsync(id, dto, cancellationToken);

        if (!updated)
            return NotFound();

        await _cache.RemoveAsync(
            $"tenant:{_tenantContext.TenantId}:speakers:all",
            cancellationToken);

        await _cache.RemoveAsync(
            $"tenant:{_tenantContext.TenantId}:speaker:{id}",
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Permission:DeleteSpeakers")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var deleted = await _speakerService.DeleteAsync(id, cancellationToken);

        if (!deleted)
            return NotFound();

        await _cache.RemoveAsync(
            $"tenant:{_tenantContext.TenantId}:speakers:all",
            cancellationToken);

        await _cache.RemoveAsync(
            $"tenant:{_tenantContext.TenantId}:speaker:{id}",
            cancellationToken);

        return NoContent();
    }
}

