namespace Eventix.Application.DTOs.UserRoles;

public class UserRoleResponseDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public DateTime AssignedAt { get; set; }
}