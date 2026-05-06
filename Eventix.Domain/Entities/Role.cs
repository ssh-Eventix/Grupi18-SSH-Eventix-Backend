using System.Collections.Generic;
using Eventix.Domain.Common;

namespace Eventix.Domain.Entities
{

    public class Role : TenantBaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        // If true, the role is global (applies across all tenants)
        public bool IsGlobal { get; set; } = false;
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    }

}