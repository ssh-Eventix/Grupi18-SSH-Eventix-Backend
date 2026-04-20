namespace Eventix.Application.DTOs.Tenants;

public class CreateTenantDto
{
    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string? Domain { get; set; }
}