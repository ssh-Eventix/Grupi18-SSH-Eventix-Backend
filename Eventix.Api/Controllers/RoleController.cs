using Eventix.Application.DTOs.Roles;
using Eventix.Application.Interfaces.Services;
using Eventix.Application.Interfaces.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

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
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult<IEnumerable<RoleResponseDTO>>> GetAll(CancellationToken cancellationToken)
        {
            var roles = await _service.GetAllAsync(cancellationToken);

            return Ok(roles);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult<RoleResponseDTO>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var dto = await _service.GetByIdAsync(id, cancellationToken);
            if (dto is null)
                return NotFound();

            return Ok(dto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult<RoleResponseDTO>> Create([FromBody] CreateRoleDTO dto, CancellationToken cancellationToken)
        {
            var response = await _service.CreateAsync(dto, _tenantContext.TenantId, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoleDTO dto, CancellationToken cancellationToken)
        {
            var existing = await _service.GetByIdAsync(id, cancellationToken);

            var updated = await _service.UpdateAsync(id, dto, cancellationToken);
            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var existing = await _service.GetByIdAsync(id, cancellationToken);

            var deleted = await _service.DeleteAsync(id, cancellationToken);
            return deleted ? NoContent() : NotFound();
        }
    }
}
