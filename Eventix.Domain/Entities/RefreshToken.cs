using Eventix.Domain.Common;

namespace Eventix.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public Guid PublicUserId { get; set; }

    public PublicUser PublicUser { get; set; } = null!;

    public string TokenHash { get; set; } = string.Empty;
    
    public DateTime ExpiresAtUtc { get; set; }

    public DateTime? RevokedAtUtc { get; set; }

    public string? ReplacedByTokenHash { get; set; }

    public bool IsRevoked => RevokedAtUtc != null;

    public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;

    public bool IsActive => !IsRevoked && !IsExpired;
}