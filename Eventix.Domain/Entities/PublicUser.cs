using Eventix.Domain.Common;

namespace Eventix.Domain.Entities
{
    public class PublicUser : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; 
        public string FullName { get; set; } = string.Empty;
        public bool IsSuperAdmin { get; set; } = false;
        public bool IsActive { get; set; } = true;

        public ICollection<PublicUserRole> PublicUserRoles { get; set; } = new List<PublicUserRole>();
    }
}

