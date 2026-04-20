namespace Eventix.Application.Interfaces.Services;

public interface ITenantSchemaProvisioner
{
    Task ProvisionTenantSchemaAsync(string schemaName, CancellationToken cancellationToken = default);
}