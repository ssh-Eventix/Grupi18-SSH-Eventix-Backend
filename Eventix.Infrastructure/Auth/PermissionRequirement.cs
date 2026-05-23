using Eventix.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace Eventix.Api.Authorization;

public class PermissionRequirement : IAuthorizationRequirement
{
    public Permission Permission { get; }

    public PermissionRequirement(Permission permission)
    {
        Permission = permission;
    }
}