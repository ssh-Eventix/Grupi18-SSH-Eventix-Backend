using System.Security.Claims; 
using Eventix.Application.Interfaces.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Eventix.Infrastructure.Auth;

public class TenantAdminHandler : AuthorizationHandler<TenantAdminRequirement>
{
    private readonly ITenantContext _tenantContext;
    private readonly ILogger<TenantAdminHandler> _logger;

    public TenantAdminHandler(ITenantContext tenantContext, ILogger<TenantAdminHandler> logger)
    {
        _tenantContext = tenantContext;
        _logger = logger;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TenantAdminRequirement requirement)
    {
        var user = context.User;
        if (user == null)
        {
            return Task.CompletedTask;
        }

        if (user.Identity == null || !user.Identity.IsAuthenticated)
        {
            _logger.LogDebug("Authorization failed: principal is not authenticated.");
            return Task.CompletedTask;
        }

        // 1) If the user has an isSuperAdmin claim, allow immediately (platform-level override)
        if (user.HasClaim(c => c.Type == "isSuperAdmin" && c.Value == "true"))
        {
            _logger.LogDebug("Authorization succeeded because user has isSuperAdmin claim.");
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // Legacy globalRole claims removed; rely on explicit isSuperAdmin claim only

        // 2) Otherwise, verify tenant admin membership
        // Ensure token tenantId matches current tenant context
        var tenantIdClaim = user.FindFirst("tenantId") ?? user.FindFirst("tenant_id");
        if (tenantIdClaim == null)
        {
            _logger.LogDebug("Authorization failed: token does not contain tenantId.");
            return Task.CompletedTask;
        }

        if (!System.Guid.TryParse(tenantIdClaim.Value, out var tokenTenantId))
        {
            _logger.LogDebug("Authorization failed: tenantId claim is not a valid GUID: {Value}", tenantIdClaim.Value);
            return Task.CompletedTask;
        }

        if (tokenTenantId != _tenantContext.TenantId)
        {
            _logger.LogDebug("Authorization failed: token tenantId {TokenTenant} does not match request tenant {RequestTenant}.", tokenTenantId, _tenantContext.TenantId);
            return Task.CompletedTask;
        }

        // Check if the user has the tenant Admin role
        // Prefer ClaimTypes.Role but also check plain "role" claims
        var isAdmin = user.IsInRole("Admin") || user.HasClaim(c =>
            (c.Type == ClaimTypes.Role || string.Equals(c.Type, "role", System.StringComparison.OrdinalIgnoreCase))
            && string.Equals(c.Value, "Admin", System.StringComparison.OrdinalIgnoreCase));

        if (isAdmin)
        {
            _logger.LogDebug("Authorization succeeded: user is tenant Admin for tenant {Tenant}.", _tenantContext.TenantId);
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        _logger.LogDebug("Authorization failed: user is not tenant Admin and not SuperAdmin.");
        return Task.CompletedTask;
    }
}

