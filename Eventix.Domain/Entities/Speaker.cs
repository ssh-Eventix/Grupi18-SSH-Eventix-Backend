using System;
using Eventix.Domain.Common;
namespace Eventix.Domain.Entities;

public class Speaker : TenantBaseEntity
{
    public string FullName { get; set; } = string.Empty;

    public string? Bio { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? ProfileImageUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<EventSession> Sessions { get; set; } = new List<EventSession>();
}