using Microsoft.AspNetCore.Authorization;

namespace Eventix.Infrastructure.Auth;

public class TenantAdminRequirement : IAuthorizationRequirement
{
    // Marker requirement for Tenant admin or SuperAdmin global role
}

