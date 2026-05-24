namespace Eventix.Application.DTOs.CheckIns;

public class CreateCheckInDTO
{
    public Guid TicketId { get; set; }
    public Guid CheckedInByUserId { get; set; }
    public string? Notes { get; set; }
}