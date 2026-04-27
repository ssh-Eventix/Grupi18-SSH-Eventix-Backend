namespace Eventix.Application.DTOs.Tenants;

using Eventix.Domain.Enums;

public class CreateTenantDTO
{
    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string SchemaName { get; set; } = default!;

    public string? Description { get; set; }
    public string? ContactEmail { get; set; }

    public string? City { get; set; }
    public string? Country { get; set; }

    public string? LogoUrl { get; set; }

    public TenantStatus Status { get; set; } = TenantStatus.Active;
    public bool IsTrial { get; set; } = false;
    public bool IsActive { get; set; } = true;
}