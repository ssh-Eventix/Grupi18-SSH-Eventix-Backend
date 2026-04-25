using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Eventix.Application.DTOs.Roles;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Application.Interfaces.Common;
using Microsoft.AspNetCore.Mvc;

namespace Eventix.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleRepository _repository;
        private readonly ITenantContext _tenantContext;

        public RoleController(IRoleRepository repository, ITenantContext tenantContext)
        {
            _repository = repository;
            _tenantContext = tenantContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleResponseDTO>>> GetAll(CancellationToken cancellationToken)
        {
            var roles = await _repository.GetAllAsync(cancellationToken);
            var response = roles
                .Where(r => r.TenantId == _tenantContext.TenantId && !r.IsDeleted)
                .Select(r => new RoleResponseDTO
                {
                    Id = r.Id,
                    TenantId = r.TenantId,
                    Name = r.Name,
                    Description = r.Description,
                    CreatedAtUtc = r.CreatedAtUtc,
                    UpdatedAtUtc = r.UpdatedAtUtc
                })
                .ToList();

            return Ok(response);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<RoleResponseDTO>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var role = await _repository.GetByIdAsync(id, cancellationToken);
            if (role is null || role.TenantId != _tenantContext.TenantId || role.IsDeleted)
                return NotFound();

            var dto = new RoleResponseDTO
            {
                Id = role.Id,
                TenantId = role.TenantId,
                Name = role.Name,
                Description = role.Description,
                CreatedAtUtc = role.CreatedAtUtc,
                UpdatedAtUtc = role.UpdatedAtUtc
            };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<RoleResponseDTO>> Create([FromBody] CreateRoleDTO dto, CancellationToken cancellationToken)
        {
            var entity = new Role
            {
                Id = Guid.NewGuid(),
                TenantId = _tenantContext.TenantId,
                Name = dto.Name,
                Description = dto.Description
            };

            await _repository.AddAsync(entity, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            var response = new RoleResponseDTO
            {
                Id = entity.Id,
                TenantId = entity.TenantId,
                Name = entity.Name,
                Description = entity.Description,
                CreatedAtUtc = entity.CreatedAtUtc
            };

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, response);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoleDTO dto, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(id, cancellationToken);
            if (entity is null || entity.TenantId != _tenantContext.TenantId || entity.IsDeleted)
                return NotFound();

            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync(cancellationToken);

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(id, cancellationToken);
            if (entity is null || entity.TenantId != _tenantContext.TenantId || entity.IsDeleted)
                return NotFound();

            entity.IsDeleted = true;
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync(cancellationToken);

            return NoContent();
        }
    }
}

