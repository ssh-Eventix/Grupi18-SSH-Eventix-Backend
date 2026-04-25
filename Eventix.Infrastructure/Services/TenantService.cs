using System.Text.RegularExpressions;
using Eventix.Application.DTOs.Tenants;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;
using Eventix.Domain.Enums;

namespace Eventix.Infrastructure.Services;

public class TenantService : ITenantService
{
    private readonly ITenantRepository _repository;
    private readonly ITenantSchemaProvisioner _schemaProvisioner;

    public TenantService(
        ITenantRepository repository,
        ITenantSchemaProvisioner schemaProvisioner)
    {
        _repository = repository;
        _schemaProvisioner = schemaProvisioner;
    }

    public async Task<IReadOnlyList<TenantResponseDTO>> GetAllAsync(CancellationToken ct)
    {
        var tenants = await _repository.GetAllAsync(ct);
        return tenants.Select(Map).ToList();
    }

    public async Task<TenantResponseDTO?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var tenant = await _repository.GetByIdAsync(id, ct);
        return tenant is null ? null : Map(tenant);
    }

    public async Task<TenantResponseDTO?> GetBySlugAsync(string slug, CancellationToken ct)
    {
        var tenant = await _repository.GetBySlugAsync(slug, ct);
        return tenant is null ? null : Map(tenant);
    }

    public async Task<TenantResponseDTO> CreateAsync(CreateTenantDTO dto, CancellationToken ct)
    {
        var schemaName = $"tenant_{dto.Slug}";

        var entity = new Tenant
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Slug = dto.Slug,
            SchemaName = schemaName,
            Description = dto.Description,
            ContactEmail = dto.ContactEmail,
            City = dto.City,
            Country = dto.Country,
            LogoUrl = dto.LogoUrl,
            IsTrial = dto.IsTrial,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _repository.AddAsync(entity, ct);
        await _repository.SaveChangesAsync(ct);

        await _schemaProvisioner.ProvisionTenantSchemaAsync(schemaName, ct);

        return Map(entity);
    }

    public async Task<TenantResponseDTO?> UpdateAsync(Guid id, UpdateTenantDTO dto, CancellationToken ct)
    {
        var tenant = await _repository.GetByIdAsync(id, ct);
        if (tenant is null) return null;

        tenant.Name = dto.Name;
        tenant.Description = dto.Description;
        tenant.ContactEmail = dto.ContactEmail;
        tenant.City = dto.City;
        tenant.Country = dto.Country;
        tenant.LogoUrl = dto.LogoUrl;
        tenant.IsActive = dto.IsActive;

        await _repository.UpdateAsync(tenant);
        await _repository.SaveChangesAsync(ct);

        return Map(tenant);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var tenant = await _repository.GetByIdAsync(id, ct);
        if (tenant is null) return false;

        await _repository.DeleteAsync(tenant);
        await _repository.SaveChangesAsync(ct);

        return true;
    }

    private static TenantResponseDTO Map(Tenant t)
        => new()
        {
            Id = t.Id,
            Name = t.Name,
            Slug = t.Slug,
            SchemaName = t.SchemaName,
            Description = t.Description,
            ContactEmail = t.ContactEmail,
            City = t.City,
            Country = t.Country,
            LogoUrl = t.LogoUrl,
            Status = t.Status.ToString(),
            IsTrial = t.IsTrial,
            IsActive = t.IsActive,
            CreatedAtUtc = t.CreatedAtUtc
        };
}