using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Eventix.Infrastructure.Auth;

public class SuperAdminImpersonationHandler : AuthorizationHandler<SuperAdminImpersonationRequirement>
{
    private readonly ILogger<SuperAdminImpersonationHandler> _logger;

    public SuperAdminImpersonationHandler(ILogger<SuperAdminImpersonationHandler> logger)
    {
        _logger = logger;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SuperAdminImpersonationRequirement requirement)
    {
        var user = context.User;
        if (user == null)
            return Task.CompletedTask;

        if (user.HasClaim(c => c.Type == "isSuperAdmin" && c.Value == "true"))
        {
            _logger.LogDebug("SuperAdmin impersonation allowed via isSuperAdmin claim.");
            context.Succeed(requirement);
            return Task.CompletedTask;
        }


        _logger.LogDebug("SuperAdmin impersonation denied: caller is not SuperAdmin.");
        return Task.CompletedTask;
    }
}

