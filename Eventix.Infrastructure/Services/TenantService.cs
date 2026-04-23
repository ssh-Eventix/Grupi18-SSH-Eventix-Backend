using System.Text.RegularExpressions;
using Eventix.Application.DTOs.Tenants;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;
using Eventix.Domain.Enums;

namespace Eventix.Infrastructure.Services;

public class TenantService : ITenantService
{
    private readonly ITenantRepository _tenantRepository;
    private readonly ITenantSchemaProvisioner _tenantSchemaProvisioner;

    public TenantService(
        ITenantRepository tenantRepository,
        ITenantSchemaProvisioner tenantSchemaProvisioner)
    {
        _tenantRepository = tenantRepository;
        _tenantSchemaProvisioner = tenantSchemaProvisioner;
    }

    public async Task<IReadOnlyList<TenantResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var tenants = await _tenantRepository.GetAllAsync(cancellationToken);
        return tenants.Select(MapToResponseDto).ToList();
    }

    public async Task<TenantResponseDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tenant = await _tenantRepository.GetByIdAsync(id, cancellationToken);
        return tenant is null ? null : MapToResponseDto(tenant);
    }

    public async Task<TenantResponseDTO?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var normalizedSlug = NormalizeSlug(slug);

        var tenant = await _tenantRepository.GetBySlugAsync(normalizedSlug, cancellationToken);
        return tenant is null ? null : MapToResponseDto(tenant);
    }

    public async Task<TenantResponseDTO> CreateAsync(CreateTenantDTO dto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Tenant name is required.");

        if (string.IsNullOrWhiteSpace(dto.Slug))
            throw new ArgumentException("Tenant slug is required.");

        var normalizedSlug = NormalizeSlug(dto.Slug);

        if (!IsValidSlug(normalizedSlug))
            throw new ArgumentException("Slug can contain only lowercase letters, numbers, and hyphens.");

        var exists = await _tenantRepository.ExistsBySlugAsync(normalizedSlug, cancellationToken);
        if (exists)
            throw new InvalidOperationException("Tenant with this slug already exists.");

        var tenant = new Tenant
        {
            Name = dto.Name.Trim(),
            Slug = normalizedSlug,
            SchemaName = $"tenant_{normalizedSlug}",

            Description = dto.Description,

            ContactEmail = dto.ContactEmail,
            ContactPhone = dto.ContactPhone,

            AddressLine1 = dto.AddressLine1,
            AddressLine2 = dto.AddressLine2,
            City = dto.City,
            State = dto.State,
            PostalCode = dto.PostalCode,
            Country = dto.Country,

            LogoUrl = dto.LogoUrl,
            WebsiteUrl = dto.WebsiteUrl,

            MaxUsers = dto.MaxUsers,
            MaxEvents = dto.MaxEvents,
            IsTrial = dto.IsTrial,

            Status = TenantStatus.Active,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _tenantRepository.AddAsync(tenant, cancellationToken);
        await _tenantSchemaProvisioner.ProvisionTenantSchemaAsync(tenant.SchemaName, cancellationToken);

        return MapToResponseDto(tenant);
    }

    public async Task<TenantResponseDTO?> UpdateAsync(Guid id, UpdateTenantDTO dto, CancellationToken cancellationToken = default)
    {
        var tenant = await _tenantRepository.GetByIdAsync(id, cancellationToken);
        if (tenant is null)
            return null;

        tenant.Name = dto.Name.Trim();
        tenant.Description = dto.Description;

        tenant.ContactEmail = dto.ContactEmail;
        tenant.ContactPhone = dto.ContactPhone;

        tenant.AddressLine1 = dto.AddressLine1;
        tenant.AddressLine2 = dto.AddressLine2;
        tenant.City = dto.City;
        tenant.State = dto.State;
        tenant.PostalCode = dto.PostalCode;
        tenant.Country = dto.Country;

        tenant.LogoUrl = dto.LogoUrl;
        tenant.WebsiteUrl = dto.WebsiteUrl;

        tenant.IsActive = dto.IsActive;
        tenant.MaxUsers = dto.MaxUsers;
        tenant.MaxEvents = dto.MaxEvents;
        tenant.UpdatedAtUtc = DateTime.UtcNow;

        await _tenantRepository.UpdateAsync(tenant, cancellationToken);

        return MapToResponseDto(tenant);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tenant = await _tenantRepository.GetByIdAsync(id, cancellationToken);
        if (tenant is null)
            return false;

        await _tenantRepository.DeleteAsync(tenant, cancellationToken);
        return true;
    }

    private static string NormalizeSlug(string slug)
        => slug.Trim().ToLowerInvariant();

    private static bool IsValidSlug(string slug)
        => Regex.IsMatch(slug, "^[a-z0-9-]+$");

    private static TenantResponseDTO MapToResponseDto(Tenant tenant)
    {
        return new TenantResponseDTO
        {
            Id = tenant.Id,
            Name = tenant.Name,
            Slug = tenant.Slug,
            SchemaName = tenant.SchemaName,
            Description = tenant.Description,
            ContactEmail = tenant.ContactEmail,
            ContactPhone = tenant.ContactPhone,
            City = tenant.City,
            Country = tenant.Country,
            LogoUrl = tenant.LogoUrl,
            WebsiteUrl = tenant.WebsiteUrl,
            Status = tenant.Status.ToString(),
            IsTrial = tenant.IsTrial,
            IsActive = tenant.IsActive,
            CreatedAtUtc = tenant.CreatedAtUtc
        };
    }
}