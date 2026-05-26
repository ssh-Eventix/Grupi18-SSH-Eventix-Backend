using Eventix.Domain.Common;

namespace Eventix.Domain.Entities
{
    public class PublicUser : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; 
        public string FullName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        public DateTime? LastLoginAtUtc { get; set; }
        public ICollection<PublicUserRole> PublicUserRoles { get; set; } = new List<PublicUserRole>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = new List<PasswordResetToken>();
    }
}

