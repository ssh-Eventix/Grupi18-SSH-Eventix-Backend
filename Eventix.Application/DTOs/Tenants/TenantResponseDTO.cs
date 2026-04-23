namespace Eventix.Application.DTOs.Tenants;

public class TenantResponseDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string SchemaName { get; set; } = default!;
    public string? Description { get; set; }

    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }

    public string? City { get; set; }
    public string? Country { get; set; }

    public string? LogoUrl { get; set; }
    public string? WebsiteUrl { get; set; }

    public string Status { get; set; } = default!;
    public bool IsTrial { get; set; }
    public bool IsActive { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}