using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.MultiTenancy;
using Microsoft.AspNetCore.Mvc;

namespace Eventix.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _repository;
        private readonly ITenantContext _tenantContext;

        public UserController(IUserRepository repository, ITenantContext tenantContext)
        {
            _repository = repository;
            _tenantContext = tenantContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAll(CancellationToken cancellationToken)
        {
            var users = await _repository.GetAllAsync(cancellationToken);
            var response = users
                .Where(u => u.TenantId == _tenantContext.TenantId && !u.IsDeleted)
                .ToList();

            return Ok(response);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<User>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var user = await _repository.GetByIdAsync(id, cancellationToken);
            if (user is null || user.TenantId != _tenantContext.TenantId || user.IsDeleted)
                return NotFound();

            return Ok(user);
        }

        [HttpGet("by-email")]
        public async Task<ActionResult<User>> GetByEmail([FromQuery] string email, CancellationToken cancellationToken)
        {
            var user = await _repository.GetByEmailAsync(email, cancellationToken);
            if (user is null || user.TenantId != _tenantContext.TenantId || user.IsDeleted)
                return NotFound();

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<User>> Create([FromBody] User dto, CancellationToken cancellationToken)
        {
            // Note: dto.PasswordHash should contain the already-hashed password.
            var entity = new User
            {
                Id = Guid.NewGuid(),
                TenantId = _tenantContext.TenantId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = dto.PasswordHash,
                IsActive = dto.IsActive,
                CreatedAtUtc = DateTime.UtcNow
            };

            await _repository.AddAsync(entity, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] User dto, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(id, cancellationToken);
            if (entity is null || entity.TenantId != _tenantContext.TenantId || entity.IsDeleted)
                return NotFound();

            entity.FirstName = dto.FirstName;
            entity.LastName = dto.LastName;
            entity.Email = dto.Email;
            // If you want to update password, set PasswordHash explicitly
            if (!string.IsNullOrWhiteSpace(dto.PasswordHash))
                entity.PasswordHash = dto.PasswordHash;

            entity.IsActive = dto.IsActive;
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

