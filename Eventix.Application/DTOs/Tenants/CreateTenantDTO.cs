namespace Eventix.Application.DTOs.Tenants;

public class CreateTenantDTO
{
    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string? Description { get; set; }

    public string? ContactEmail { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }

    public string? LogoUrl { get; set; }
    public bool IsTrial { get; set; } = false;
}