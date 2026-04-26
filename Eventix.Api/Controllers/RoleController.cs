using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Eventix.Application.DTOs.Roles;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;
using Eventix.Application.Interfaces.Common;
using Microsoft.AspNetCore.Mvc;

namespace Eventix.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
            private readonly IRoleService _service;
            private readonly ITenantContext _tenantContext;

            public RoleController(IRoleService service, ITenantContext tenantContext)
            {
                _service = service;
                _tenantContext = tenantContext;
            }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleResponseDTO>>> GetAll(CancellationToken cancellationToken)
        {
            var roles = await _service.GetAllAsync(cancellationToken);
            var response = roles
                .Where(r => r.TenantId == _tenantContext.TenantId)
                .ToList();

            return Ok(response);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<RoleResponseDTO>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var dto = await _service.GetByIdAsync(id, cancellationToken);
            if (dto is null || dto.TenantId != _tenantContext.TenantId)
                return NotFound();

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<RoleResponseDTO>> Create([FromBody] CreateRoleDTO dto, CancellationToken cancellationToken)
        {
            var response = await _service.CreateAsync(dto, _tenantContext.TenantId, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoleDTO dto, CancellationToken cancellationToken)
        {
            var existing = await _service.GetByIdAsync(id, cancellationToken);
            if (existing is null || existing.TenantId != _tenantContext.TenantId)
                return NotFound();

            var updated = await _service.UpdateAsync(id, dto, cancellationToken);
            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var existing = await _service.GetByIdAsync(id, cancellationToken);
            if (existing is null || existing.TenantId != _tenantContext.TenantId)
                return NotFound();

            var deleted = await _service.DeleteAsync(id, cancellationToken);
            return deleted ? NoContent() : NotFound();
        }
    }
}

