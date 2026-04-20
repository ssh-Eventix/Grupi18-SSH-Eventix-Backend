using Eventix.Domain.Common;

namespace Eventix.Domain.Entities;

public class Tenant : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string SchemaName { get; set; } = default!;
    public string? Domain { get; set; }
    public bool IsActive { get; set; } = true;
}