using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.MultiTenancy;
using Microsoft.AspNetCore.Mvc;

namespace Eventix.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VenueSectionsController : ControllerBase
{
    private readonly IVenueSectionRepository _repository;
    private readonly ITenantContext _tenantContext;

    public VenueSectionsController(IVenueSectionRepository repository, ITenantContext tenantContext)
    {
        _repository = repository;
        _tenantContext = tenantContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => Ok(await _repository.GetAllAsync(cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return NotFound();
        return Ok(entity);
    }

    [HttpGet("by-venue/{venueId:guid}")]
    public async Task<IActionResult> GetByVenue(Guid venueId, CancellationToken cancellationToken)
        => Ok(await _repository.GetByVenueIdAsync(venueId, cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] VenueSection entity, CancellationToken cancellationToken)
    {
        entity.TenantId = _tenantContext.TenantId!.Value;

        await _repository.AddAsync(entity, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] VenueSection model, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return NotFound();

        entity.Name = model.Name;
        entity.Capacity = model.Capacity;
        entity.Description = model.Description;
        entity.VenueId = model.VenueId;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _repository.UpdateAsync(entity);
        await _repository.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return NotFound();

        await _repository.DeleteAsync(entity);
        await _repository.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}