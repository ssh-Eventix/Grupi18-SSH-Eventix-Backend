using Eventix.Domain.Common;

namespace Eventix.Domain.Entities;

public class PublicRole : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public ICollection<PublicUserRole> PublicUserRoles { get; set; } = new List<PublicUserRole>();
}