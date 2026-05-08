namespace Eventix.Application.DTOs.Notifications;

public class CreateNotificationDTO
{
    public Guid UserId { get; set; }
    public Guid? EventId { get; set; }
    public int Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}