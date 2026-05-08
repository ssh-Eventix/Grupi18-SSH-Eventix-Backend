namespace Eventix.Application.DTOs.CheckIns;

public class CheckInDto
{
    public Guid Id { get; set; }
    public Guid TicketId { get; set; }
    public Guid CheckedInByUserId { get; set; }
    public DateTime CheckInTime { get; set; }
    public string? Notes { get; set; }
}