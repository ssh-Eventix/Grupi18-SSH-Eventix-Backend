using Eventix.Application.DTOs.Tenants;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Eventix.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TenantsController : ControllerBase
{
    private readonly ITenantRepository _tenantRepository;
    private readonly ITenantSchemaProvisioner _tenantSchemaProvisioner;

    public TenantsController(
        ITenantRepository tenantRepository,
        ITenantSchemaProvisioner tenantSchemaProvisioner)
    {
        _tenantRepository = tenantRepository;
        _tenantSchemaProvisioner = tenantSchemaProvisioner;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var tenants = await _tenantRepository.GetAllAsync(cancellationToken);
        return Ok(tenants);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var tenant = await _tenantRepository.GetByIdAsync(id, cancellationToken);
        if (tenant is null) return NotFound();

        return Ok(tenant);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTenantDto dto, CancellationToken cancellationToken)
    {
        var slug = dto.Slug.Trim().ToLower();
        var schemaName = $"tenant_{slug.Replace("-", "_")}";

        var existing = await _tenantRepository.GetBySlugAsync(slug, cancellationToken);
        if (existing is not null)
            return BadRequest(new { message = "Tenant slug already exists." });

        var tenant = new Tenant
        {
            Name = dto.Name,
            Slug = slug,
            SchemaName = schemaName,
            Domain = dto.Domain,
            IsActive = true
        };

        await _tenantRepository.AddAsync(tenant, cancellationToken);
        await _tenantRepository.SaveChangesAsync(cancellationToken);

        await _tenantSchemaProvisioner.ProvisionTenantSchemaAsync(schemaName, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = tenant.Id }, tenant);
    }
}