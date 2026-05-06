namespace Eventix.Infrastructure.Auth
{
    public interface IRolePermissionService
    {
        bool RoleHasPermission(string roleName, Permission permission);
        IEnumerable<Permission> GetPermissionsForRole(string roleName);
    }
}

