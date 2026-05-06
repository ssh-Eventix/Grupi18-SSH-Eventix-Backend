namespace Eventix.Infrastructure.Auth
{
    public class RolePermissionService : IRolePermissionService
    {
        private readonly Dictionary<string, HashSet<Permission>> _map;

        public RolePermissionService()
        {
            _map = new Dictionary<string, HashSet<Permission>>(StringComparer.OrdinalIgnoreCase)
            {
                ["SuperAdmin"] = Enum.GetValues(typeof(Permission)).Cast<Permission>().ToHashSet(),

                ["Buyer"] = new HashSet<Permission>
                {
                    Permission.EventsRead,
                    Permission.TicketsRead,
                    Permission.TicketsPurchase,
                    Permission.VenuesRead
                },

                // Staff: example role with event/venue management but not user assignment
                ["Admin"] = new HashSet<Permission>
                {
                    Permission.EventsCreate, Permission.EventsRead, Permission.EventsUpdate, Permission.EventsDelete,
                    Permission.VenuesRead, Permission.VenuesUpdate, Permission.VenuesCreate
                }
            };
        }

        public bool RoleHasPermission(string roleName, Permission permission)
        {
            if (string.IsNullOrWhiteSpace(roleName)) return false;
            
            if (string.Equals(roleName, "Admin", StringComparison.OrdinalIgnoreCase))
            { 
                return PermissionExtensions.IsAdminGrantedByDefault(permission);
            }

            if (!_map.TryGetValue(roleName, out var perms)) return false;
            return perms.Contains(permission);
        }

        public IEnumerable<Permission> GetPermissionsForRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName)) return Enumerable.Empty<Permission>();
            if (!_map.TryGetValue(roleName, out var perms)) return Enumerable.Empty<Permission>();
            return perms;
        }
    }
}

