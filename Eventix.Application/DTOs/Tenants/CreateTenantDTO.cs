namespace Eventix.Application.DTOs.Tenants;

public class CreateTenantDTO
{
    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string? Description { get; set; }

    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }

    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }

    public string? LogoUrl { get; set; }
    public string? WebsiteUrl { get; set; }

    public int MaxUsers { get; set; } = 10;
    public int MaxEvents { get; set; } = 100;
    public bool IsTrial { get; set; } = false;
}