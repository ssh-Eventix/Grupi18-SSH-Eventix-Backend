using System;
using System.Threading.Tasks;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;

namespace Eventix.Infrastructure.Services;

public class TenantService : ITenantService
{
    private readonly PublicDbContext _publicDbContext;
    private readonly ITenantSchemaProvisioner _schemaProvisioner;

    public TenantService(
        PublicDbContext publicDbContext,
        ITenantSchemaProvisioner schemaProvisioner)
    {
        _publicDbContext = publicDbContext;
        _schemaProvisioner = schemaProvisioner;
    }

    public async Task<Guid> CreateTenantAsync(string name, string slug)
    {
        var schemaName = $"tenant_{slug}";

        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Name = name,
            Slug = slug,
            SchemaName = schemaName,
            Domain = null,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        _publicDbContext.Tenants.Add(tenant);
        await _publicDbContext.SaveChangesAsync();

        await _schemaProvisioner.ProvisionTenantSchemaAsync(schemaName);

        return tenant.Id;
    }
}