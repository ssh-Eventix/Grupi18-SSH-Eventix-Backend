using Eventix.Domain.Common;

namespace Eventix.Domain.Entities;

public class PublicUserRole 
{
    public Guid PublicUserId { get; set; }

    public PublicUser PublicUser { get; set; } = null!;

    public Guid PublicRoleId { get; set; }

    public PublicRole PublicRole { get; set; } = null!;
}