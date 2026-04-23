namespace Eventix.Application.DTOs.Tenants;

public class UpdateTenantDTO
{
    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string? Domain { get; set; }
    public bool IsActive { get; set; }
}