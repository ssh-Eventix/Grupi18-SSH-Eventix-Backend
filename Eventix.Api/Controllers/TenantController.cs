using Eventix.Application.DTOs.Tenants;
using Eventix.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Eventix.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TenantsController : ControllerBase
{
    private readonly ITenantService _tenantService;

    public TenantsController(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var tenants = await _tenantService.GetAllAsync(cancellationToken);
        return Ok(tenants);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var tenant = await _tenantService.GetByIdAsync(id, cancellationToken);

        if (tenant is null)
            return NotFound();

        return Ok(tenant);
    }

    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetBySlug(string slug, CancellationToken cancellationToken)
    {
        var tenant = await _tenantService.GetBySlugAsync(slug, cancellationToken);

        if (tenant is null)
            return NotFound();

        return Ok(tenant);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTenantDTO dto, CancellationToken cancellationToken)
    {
        try
        {
            var createdTenant = await _tenantService.CreateAsync(dto, cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdTenant.Id },
                createdTenant);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTenantDTO dto, CancellationToken cancellationToken)
    {
        var updatedTenant = await _tenantService.UpdateAsync(id, dto, cancellationToken);

        if (updatedTenant is null)
            return NotFound();

        return Ok(updatedTenant);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _tenantService.DeleteAsync(id, cancellationToken);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}