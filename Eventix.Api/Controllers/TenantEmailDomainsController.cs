using Eventix.Application.DTOs.TenantEmailDomains;
using Eventix.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eventix.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TenantEmailDomainsController : ControllerBase
    {
        private readonly ITenantEmailDomainService _service;

        public TenantEmailDomainsController(ITenantEmailDomainService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Policy = "Permission:ViewTenantEmailDomains")]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            return Ok(await _service.GetAllAsync(ct));
        }

        [HttpGet("tenant/{tenantId:guid}")]
        [Authorize(Policy = "Permission:ViewTenantEmailDomains")]
        public async Task<IActionResult> GetByTenantId(Guid tenantId, CancellationToken ct)
        {
            return Ok(await _service.GetByTenantIdAsync(tenantId, ct));
        }

        [HttpGet("{id:guid}")]
        [Authorize(Policy = "Permission:ViewTenantEmailDomains")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var result = await _service.GetByIdAsync(id, ct);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "Permission:ManageTenantEmailDomains")]
        public async Task<IActionResult> Create(CreateTenantEmailDomainDTO dto, CancellationToken ct)
        {
            var result = await _service.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "Permission:ManageTenantEmailDomains")]
        public async Task<IActionResult> Update(Guid id, UpdateTenantEmailDomainDTO dto, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, dto, ct);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "Permission:ManageTenantEmailDomains")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            var deleted = await _service.DeleteAsync(id, ct);
            return deleted ? NoContent() : NotFound();
        }
    }
}
