using Eventix.Api.Helpers;
using Eventix.Application.DTOs.Venues;
using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Services;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace Eventix.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VenueController : ControllerBase
{
    private readonly IVenueService _service;
    private readonly IDistributedCache _cache;
    private readonly ITenantContext _tenantContext;
    private readonly PublicDbContext _publicContext;

    public VenueController(
    IVenueService service,
    IDistributedCache cache,
    ITenantContext tenantContext,
    PublicDbContext publicContext)
    {
        _service = service;
        _cache = cache;
        _tenantContext = tenantContext;
        _publicContext = publicContext;
    }

    [HttpGet("public")]
    [Authorize]
    public async Task<IActionResult> GetAllPublic(
        CancellationToken cancellationToken)
    {
        var result = await _publicContext.Venues
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return Ok(result);
    }

    [HttpGet("public/{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetPublicById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var venue = await _publicContext.Venues
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return venue == null ? NotFound() : Ok(venue);
    }

    [HttpGet]
    [Authorize(Policy = "Permission:ViewVenues")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        const string cacheKey = "public:venues:all";

        var result = await CacheHelper.GetOrSetAsync(
            _cache,
            cacheKey,
            () => _service.GetAllAsync(),
            TimeSpan.FromMinutes(10),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Permission:ViewVenues")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"public:venue:{id}";

        var venue = await CacheHelper.GetOrSetAsync(
            _cache,
            cacheKey,
            () => _service.GetByIdAsync(id),
            TimeSpan.FromMinutes(10),
            cancellationToken);

        return venue == null ? NotFound() : Ok(venue);
    }

    [HttpPost]
    [Authorize(Policy = "Permission:CreateVenues")]
    public async Task<IActionResult> Create(
        CreateVenueDTO dto,
        CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(dto);

        await _cache.RemoveAsync("public:venues:all", cancellationToken);

        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Permission:UpdateVenues")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateVenueDTO dto,
        CancellationToken cancellationToken)
    {
        var updated = await _service.UpdateAsync(id, dto);

        if (!updated)
            return NotFound();

        await _cache.RemoveAsync("public:venues:all", cancellationToken);
        await _cache.RemoveAsync($"public:venue:{id}", cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Permission:DeleteVenues")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var deleted = await _service.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        await _cache.RemoveAsync("public:venues:all", cancellationToken);
        await _cache.RemoveAsync($"public:venue:{id}", cancellationToken);

        return NoContent();
    }
}