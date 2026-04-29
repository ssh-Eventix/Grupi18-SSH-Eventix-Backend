using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Eventix.Application.DTOs.Users;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;
using Eventix.Application.Interfaces.Common;
using Microsoft.AspNetCore.Mvc;

namespace Eventix.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
            private readonly IUserService _service;
            private readonly ITenantContext _tenantContext;

            public UserController(IUserService service, ITenantContext tenantContext)
            {
                _service = service;
                _tenantContext = tenantContext;
            }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponseDTO>>> GetAll(CancellationToken cancellationToken)
        {
            var users = await _service.GetAllAsync(cancellationToken);
            return Ok(users);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<UserResponseDTO>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var dto = await _service.GetByIdAsync(id, cancellationToken);
            if (dto is null)
                return NotFound();

            return Ok(dto);
        }

        [HttpGet("by-email")]
        public async Task<ActionResult<UserResponseDTO>> GetByEmail([FromQuery] string email, CancellationToken cancellationToken)
        {
            var dto = await _service.GetByEmailAsync(email, cancellationToken);
            if (dto is null)
                return NotFound();

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<UserResponseDTO>> Create([FromBody] CreateUserDTO dto, CancellationToken cancellationToken)
        {
            var response = await _service.CreateAsync(dto, _tenantContext.TenantId, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserDTO dto, CancellationToken cancellationToken)
        {
            var existing = await _service.GetByIdAsync(id, cancellationToken);
            if (existing is null)
                return NotFound();

            var updated = await _service.UpdateAsync(id, dto, cancellationToken);
            return updated ? NoContent() : NotFound();
        }
    }
}

