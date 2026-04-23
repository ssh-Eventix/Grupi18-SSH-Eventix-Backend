namespace Eventix.Application.DTOs.VenueSections;

public class VenueSectionDTO
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid VenueId { get; set; }
    public string Name { get; set; } = default!;
    public int Capacity { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}