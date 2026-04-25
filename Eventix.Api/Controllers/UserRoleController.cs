using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Eventix.Application.DTOs.UserRoles;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;
using Eventix.Application.Interfaces.Common;
using Microsoft.AspNetCore.Mvc;

namespace Eventix.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserRoleController : ControllerBase
    {
            private readonly IUserRoleService _userRoleService;
            private readonly IUserService _userService;
            private readonly IRoleService _roleService;
            private readonly ITenantContext _tenantContext;

            public UserRoleController(
                IUserRoleService userRoleService,
                IUserService userService,
                IRoleService roleService,
                ITenantContext tenantContext)
            {
                _userRoleService = userRoleService;
                _userService = userService;
                _roleService = roleService;
                _tenantContext = tenantContext;
            }

        [HttpGet("by-user/{userId:guid}")]
        public async Task<ActionResult<List<UserRoleResponseDTO>>> GetByUserId(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _userService.GetByIdAsync(userId, cancellationToken);
            if (user is null || user.TenantId != _tenantContext.TenantId)
                return NotFound();

            var roles = await _userRoleService.GetByUserIdAsync(userId, cancellationToken);
            return Ok(roles);
        }

        [HttpPost]
        public async Task<IActionResult> Assign([FromBody] CreateUserRoleDTO dto, CancellationToken cancellationToken)
        {
            var user = await _userService.GetByIdAsync(dto.UserId, cancellationToken);
            if (user is null || user.TenantId != _tenantContext.TenantId)
                return BadRequest("Invalid user");

            var role = await _roleService.GetByIdAsync(dto.RoleId, cancellationToken);
            if (role is null || role.TenantId != _tenantContext.TenantId)
                return BadRequest("Invalid role");

            var entity = await _userRoleService.AssignAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetByUserId), new { userId = dto.UserId }, entity);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var deleted = await _userRoleService.DeleteAsync(id, cancellationToken);
            return deleted ? NoContent() : NotFound();
        }
    }
}

