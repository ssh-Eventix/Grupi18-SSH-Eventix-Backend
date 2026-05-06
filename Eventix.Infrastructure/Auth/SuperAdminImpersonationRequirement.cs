using Microsoft.AspNetCore.Authorization;

namespace Eventix.Infrastructure.Auth;

public class SuperAdminImpersonationRequirement : IAuthorizationRequirement
{
    // Marker requirement: only SuperAdmin (platform) or support role may impersonate
}

