using Eventix.Domain.Common;

namespace Eventix.Domain.Entities
{

    public class Role : TenantBaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsGlobal { get; set; } = false;
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    }

}