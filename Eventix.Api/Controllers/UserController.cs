using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Eventix.Application.DTOs.Users;
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
        public async Task<ActionResult<IEnumerable<UserResponseDTO>>> GetAll(CancellationToken cancellationToken)
        {
            var users = await _repository.GetAllAsync(cancellationToken);
            var response = users
                .Where(u => u.TenantId == _tenantContext.TenantId && !u.IsDeleted)
                .Select(u => new UserResponseDTO
                {
                    Id = u.Id,
                    TenantId = u.TenantId,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    IsActive = u.IsActive,
                    CreatedAtUtc = u.CreatedAtUtc,
                    UpdatedAtUtc = u.UpdatedAtUtc
                })
                .ToList();

            return Ok(response);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<UserResponseDTO>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var user = await _repository.GetByIdAsync(id, cancellationToken);
            if (user is null || user.TenantId != _tenantContext.TenantId || user.IsDeleted)
                return NotFound();

            var dto = new UserResponseDTO
            {
                Id = user.Id,
                TenantId = user.TenantId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                IsActive = user.IsActive,
                CreatedAtUtc = user.CreatedAtUtc,
                UpdatedAtUtc = user.UpdatedAtUtc
            };

            return Ok(dto);
        }

        [HttpGet("by-email")]
        public async Task<ActionResult<UserResponseDTO>> GetByEmail([FromQuery] string email, CancellationToken cancellationToken)
        {
            var user = await _repository.GetByEmailAsync(email, cancellationToken);
            if (user is null || user.TenantId != _tenantContext.TenantId || user.IsDeleted)
                return NotFound();

            var dto = new UserResponseDTO
            {
                Id = user.Id,
                TenantId = user.TenantId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                IsActive = user.IsActive,
                CreatedAtUtc = user.CreatedAtUtc,
                UpdatedAtUtc = user.UpdatedAtUtc
            };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<UserResponseDTO>> Create([FromBody] CreateUserDTO dto, CancellationToken cancellationToken)
        {
            // Note: dto.Password should be hashed before storing. Replace this with your hashing mechanism.
            var entity = new User
            {
                Id = Guid.NewGuid(),
                TenantId = _tenantContext.TenantId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = dto.Password, // TODO: hash password before storing
                IsActive = dto.IsActive,
                CreatedAtUtc = DateTime.UtcNow
            };

            await _repository.AddAsync(entity, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            var response = new UserResponseDTO
            {
                Id = entity.Id,
                TenantId = entity.TenantId,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Email = entity.Email,
                IsActive = entity.IsActive,
                CreatedAtUtc = entity.CreatedAtUtc
            };

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, response);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserDTO dto, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(id, cancellationToken);
            if (entity is null || entity.TenantId != _tenantContext.TenantId || entity.IsDeleted)
                return NotFound();

            entity.FirstName = dto.FirstName;
            entity.LastName = dto.LastName;
            // If you want to update email, validate and set here
            // entity.Email = dto.Email;

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                // TODO: hash the new password before storing
                entity.PasswordHash = dto.Password;
            }

            entity.IsActive = dto.IsActive;
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync(cancellationToken);

            return NoContent();
        }
    }
}

