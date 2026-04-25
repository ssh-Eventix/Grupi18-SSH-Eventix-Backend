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
    public class UserRoleController : ControllerBase
    {
        private readonly IUserRoleRepository _userRoleRepo;
        private readonly IUserRepository _userRepo;
        private readonly IRoleRepository _roleRepo;
        private readonly ITenantContext _tenantContext;

        public UserRoleController(
            IUserRoleRepository userRoleRepo,
            IUserRepository userRepo,
            IRoleRepository roleRepo,
            ITenantContext tenantContext)
        {
            _userRoleRepo = userRoleRepo;
            _userRepo = userRepo;
            _roleRepo = roleRepo;
            _tenantContext = tenantContext;
        }

        [HttpGet("by-user/{userId:guid}")]
        public async Task<ActionResult<List<UserRole>>> GetByUserId(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _userRepo.GetByIdAsync(userId, cancellationToken);
            if (user is null || user.TenantId != _tenantContext.TenantId || user.IsDeleted)
                return NotFound();

            var roles = await _userRoleRepo.GetByUserIdAsync(userId, cancellationToken);
            return Ok(roles.Where(ur => !ur.IsDeleted).ToList());
        }

        public record AssignRequest(Guid UserId, Guid RoleId);

        [HttpPost]
        public async Task<IActionResult> Assign([FromBody] AssignRequest dto, CancellationToken cancellationToken)
        {
            var user = await _userRepo.GetByIdAsync(dto.UserId, cancellationToken);
            if (user is null || user.TenantId != _tenantContext.TenantId || user.IsDeleted)
                return BadRequest("Invalid user");

            var role = await _roleRepo.GetByIdAsync(dto.RoleId, cancellationToken);
            if (role is null || role.TenantId != _tenantContext.TenantId || role.IsDeleted)
                return BadRequest("Invalid role");

            if (await _userRoleRepo.ExistsAsync(dto.UserId, dto.RoleId, cancellationToken))
                return Conflict("User already has this role assigned");

            var entity = new UserRole
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                RoleId = dto.RoleId,
                AssignedAt = DateTime.UtcNow
            };

            await _userRoleRepo.AddAsync(entity, cancellationToken);
            await _userRoleRepo.SaveChangesAsync(cancellationToken);

            return CreatedAtAction(nameof(GetByUserId), new { userId = dto.UserId }, entity);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var userRole = await _userRoleRepo.GetByIdAsync(id, cancellationToken);
            if (userRole is null || userRole.IsDeleted)
                return NotFound();

            var user = await _userRepo.GetByIdAsync(userRole.UserId, cancellationToken);
            if (user is null || user.TenantId != _tenantContext.TenantId)
                return BadRequest("Tenant mismatch or invalid user");

            await _userRoleRepo.DeleteAsync(userRole);
            await _userRoleRepo.SaveChangesAsync(cancellationToken);

            return NoContent();
        }
    }
}

