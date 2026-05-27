using Eventix.Api.Helpers;
using Eventix.Application.DTOs.VenueSections;
using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;
using Eventix.Domain.Enums;
using Eventix.Infrastructure.MultiTenancy;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace Eventix.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VenueSectionController : ControllerBase
{
    private readonly IVenueSectionService _service;
    private readonly IDistributedCache _cache;
    private readonly ITenantContext _tenantContext;
    private readonly PublicDbContext _publicContext;

    public VenueSectionController(
        IVenueSectionService service,
        IDistributedCache cache,
        ITenantContext tenantContext,
        PublicDbContext publicContext)
    {
        _service = service;
        _cache = cache;
        _tenantContext = tenantContext;
        _publicContext = publicContext;
    }

    [HttpGet]
    [Authorize(Policy = "Permission:ViewVenueSections")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var cacheKey = $"tenant:{_tenantContext.TenantId}:venuesections:all";

        var result = await CacheHelper.GetOrSetAsync(
            _cache,
            cacheKey,
            () => _service.GetAllAsync(),
            TimeSpan.FromMinutes(10),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Permission:ViewVenueSections")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"tenant:{_tenantContext.TenantId}:venuesection:{id}";

        var result = await CacheHelper.GetOrSetAsync(
            _cache,
            cacheKey,
            () => _service.GetByIdAsync(id),
            TimeSpan.FromMinutes(10),
            cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("venue/{venueId:guid}")]
    [Authorize(Policy = "Permission:ViewVenueSections")]
    public async Task<IActionResult> GetByVenue(
        Guid venueId,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"tenant:{_tenantContext.TenantId}:venuesections:venue:{venueId}";

        var result = await CacheHelper.GetOrSetAsync(
            _cache,
            cacheKey,
            () => _service.GetByVenueIdAsync(venueId),
            TimeSpan.FromMinutes(10),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "Permission:ManageVenueSections")]
    public async Task<IActionResult> Create(
        CreateVenueSectionDTO dto,
        CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(dto);

        await _cache.RemoveAsync(
            $"tenant:{_tenantContext.TenantId}:venuesections:all",
            cancellationToken);
        await _cache.RemoveAsync("public:venuesections:all", cancellationToken);
        await _cache.RemoveAsync($"public:venuesections:venue:{dto.VenueId}", cancellationToken);

        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Permission:ManageVenueSections")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateVenueSectionDTO dto,
        CancellationToken cancellationToken)
    {
        var updated = await _service.UpdateAsync(id, dto);

        if (!updated)
            return NotFound();

        await _cache.RemoveAsync(
            $"tenant:{_tenantContext.TenantId}:venuesections:all",
            cancellationToken);

        await _cache.RemoveAsync(
            $"tenant:{_tenantContext.TenantId}:venuesection:{id}",
            cancellationToken);
        await _cache.RemoveAsync("public:venuesections:all", cancellationToken);
        await _cache.RemoveAsync($"public:venuesections:venue:{dto.VenueId}", cancellationToken);

        return Ok();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Permission:ManageVenueSections")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var deleted = await _service.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        await _cache.RemoveAsync(
            $"tenant:{_tenantContext.TenantId}:venuesections:all",
            cancellationToken);

        await _cache.RemoveAsync(
            $"tenant:{_tenantContext.TenantId}:venuesection:{id}",
            cancellationToken);

        await _cache.RemoveAsync("public:venuesections:all", cancellationToken);

        return NoContent();
    }

    [HttpGet("public")]
    [Authorize]
    public async Task<IActionResult> GetAllPublic(CancellationToken cancellationToken)
    {
        var result = await _publicContext.VenueSections
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return Ok(result);
    }

    [HttpGet("public/venue/{venueId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetPublicByVenue(
        Guid venueId,
        CancellationToken cancellationToken)
    {
        var result = await _publicContext.VenueSections
            .Where(x => x.VenueId == venueId)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return Ok(result);
    }
}
