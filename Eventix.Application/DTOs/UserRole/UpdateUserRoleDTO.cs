namespace Eventix.Application.DTOs.UserRoles;

public class UpdateUserRoleDTO
{
    public Guid RoleId { get; set; }
    public DateTime? AssignedAt { get; set; }
}