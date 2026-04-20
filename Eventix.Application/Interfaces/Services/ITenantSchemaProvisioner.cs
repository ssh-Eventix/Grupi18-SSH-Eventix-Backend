using System.Threading;
using System.Threading.Tasks;
namespace Eventix.Application.Interfaces.Services;

public interface ITenantSchemaProvisioner
{
    Task ProvisionTenantSchemaAsync(string schemaName, CancellationToken cancellationToken = default);
}