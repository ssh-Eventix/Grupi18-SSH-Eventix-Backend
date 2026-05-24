using Microsoft.AspNetCore.Authorization;

namespace Eventix.Api.Authorization;

public class ImpersonationHandler : AuthorizationHandler<SuperAdminImpersonationRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        SuperAdminImpersonationRequirement requirement)
    {
        var isSuperAdmin = context.User.IsInRole("SuperAdmin") ||
                           context.User.FindFirst("isSuperAdmin")?.Value == "true";

        var isCurrentlyImpersonating =
            context.User.FindFirst("isImpersonation")?.Value == "true";

        if (isSuperAdmin && !isCurrentlyImpersonating)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}