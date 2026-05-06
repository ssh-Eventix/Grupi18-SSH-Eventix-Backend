using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Eventix.Application.Interfaces.Common;

namespace Eventix.Infrastructure.Auth;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IRolePermissionService _rolePermissionService;
    private readonly ILogger<PermissionHandler> _logger;
    private readonly ITenantContext _tenantContext;

    public PermissionHandler(IRolePermissionService rolePermissionService, ILogger<PermissionHandler> logger, ITenantContext tenantContext)
    {
        _rolePermissionService = rolePermissionService;
        _logger = logger;
        _tenantContext = tenantContext;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var user = context.User;
        if (user == null || user.Identity == null || !user.Identity.IsAuthenticated)
            return Task.CompletedTask;

        if (user.HasClaim(c => c.Type == "isSuperAdmin" && c.Value == "true"))
        {
            _logger.LogDebug("Permission {Permission} granted via isSuperAdmin claim.", requirement.Permission);
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var tenantClaim = user.FindFirst("tenantId") ?? user.FindFirst("tenant_id");
        if (tenantClaim == null)
        {
            _logger.LogDebug("Permission {Permission} denied: missing tenant claim.", requirement.Permission);
            return Task.CompletedTask;
        }

        if (!System.Guid.TryParse(tenantClaim.Value, out var tokenTenantId))
        {
            _logger.LogDebug("Permission {Permission} denied: invalid tenant id claim value {Value}.", requirement.Permission, tenantClaim.Value);
            return Task.CompletedTask;
        }

        if (_tenantContext?.TenantId == null || _tenantContext.TenantId != tokenTenantId)
        {
            _logger.LogDebug("Permission {Permission} denied: token tenant {TokenTenant} does not match request tenant {RequestTenant}.", requirement.Permission, tokenTenantId, _tenantContext?.TenantId);
            return Task.CompletedTask;
        }

        // Gather role claims
        var roles = user.Claims.Where(c => c.Type == ClaimTypes.Role || string.Equals(c.Type, "role", System.StringComparison.OrdinalIgnoreCase))
                             .Select(c => c.Value)
                             .Where(v => !string.IsNullOrWhiteSpace(v))
                             .Distinct(System.StringComparer.OrdinalIgnoreCase)
                             .ToList();

        foreach (var role in roles)
        {
            if (_rolePermissionService.RoleHasPermission(role, requirement.Permission))
            {
                _logger.LogDebug("Permission {Permission} granted via role {Role}.", requirement.Permission, role);
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
        }

        _logger.LogDebug("Permission {Permission} denied. Roles checked: {Roles}", requirement.Permission, string.Join(',', roles));
        return Task.CompletedTask;
    }
}

