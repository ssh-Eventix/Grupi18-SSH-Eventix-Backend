namespace Eventix.Application.Interfaces.Services;

public interface ITenantRoleSeeder
{
    Task SeedDefaultRolesAsync(
        Guid tenantId,
        string schemaName,
        CancellationToken ct = default);
}