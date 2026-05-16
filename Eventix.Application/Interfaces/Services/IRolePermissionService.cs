using Eventix.Domain.Enums;

namespace Eventix.Application.Interfaces.Services;

public interface IRolePermissionService
{
    bool RoleHasPermission(string role, Permission permission);
    bool UserHasPermission(IEnumerable<string> roles, Permission permission);
}