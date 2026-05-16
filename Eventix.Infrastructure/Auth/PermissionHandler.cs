using Eventix.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Eventix.API.Authorization;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IRolePermissionService _rolePermissionService;

    public PermissionHandler(IRolePermissionService rolePermissionService)
    {
        _rolePermissionService = rolePermissionService;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var roles = context.User.FindAll(ClaimTypes.Role)
            .Select(x => x.Value)
            .ToList();

        if (_rolePermissionService.UserHasPermission(roles, requirement.Permission))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}