namespace Eventix.Application.Interfaces.Common
{
    public interface ITenantContext
    {
        Guid TenantId { get; set; }
        string? SchemaName { get; set; }
    }
}
