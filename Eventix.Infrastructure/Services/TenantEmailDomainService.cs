using Eventix.Application.DTOs.TenantEmailDomains;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;

namespace Eventix.Infrastructure.Services;

public class TenantEmailDomainService : ITenantEmailDomainService
{
    private readonly ITenantEmailDomainRepository _repository;
    private readonly ITenantRepository _tenantRepository;

    private static readonly HashSet<string> AllowedRoles = new(StringComparer.OrdinalIgnoreCase)
    {
        "Staff",
        "Admin"
    };

    public TenantEmailDomainService(
        ITenantEmailDomainRepository repository,
        ITenantRepository tenantRepository)
    {
        _repository = repository;
        _tenantRepository = tenantRepository;
    }

    public async Task<List<TenantEmailDomainResponseDTO>> GetAllAsync(CancellationToken ct)
    {
        var domains = await _repository.GetAllAsync(ct);
        return domains.Select(Map).ToList();
    }

    public async Task<List<TenantEmailDomainResponseDTO>> GetByTenantIdAsync(Guid tenantId, CancellationToken ct)
    {
        var domains = await _repository.GetByTenantIdAsync(tenantId, ct);
        return domains.Select(Map).ToList();
    }

    public async Task<TenantEmailDomainResponseDTO?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var domain = await _repository.GetByIdAsync(id, ct);
        return domain is null ? null : Map(domain);
    }

    public async Task<TenantEmailDomainResponseDTO> CreateAsync(CreateTenantEmailDomainDTO dto, CancellationToken ct)
    {
        var tenant = await _tenantRepository.GetByIdAsync(dto.TenantId, ct);
        if (tenant is null)
            throw new InvalidOperationException("Tenant not found.");

        var domain = NormalizeDomain(dto.Domain);
        var role = NormalizeRole(dto.DefaultRoleName);

        var existingForTenant = await _repository.GetAnyByTenantIdAndDomainAsync(dto.TenantId, domain, ct);

        if (existingForTenant is not null)
        {
            existingForTenant.Domain = domain;
            existingForTenant.DefaultRoleName = role;
            existingForTenant.AutoApprove = dto.AutoApprove;
            existingForTenant.IsDeleted = false;
            existingForTenant.UpdatedAtUtc = DateTime.UtcNow;

            await _repository.UpdateAsync(existingForTenant, ct);
            await _repository.SaveChangesAsync(ct);
            return Map(existingForTenant);
        }

        var existingActiveDomain = await _repository.GetByDomainAsync(domain, ct);
        if (existingActiveDomain is not null && existingActiveDomain.TenantId != dto.TenantId)
            throw new InvalidOperationException("This email domain is already used by another tenant.");

        var entity = new TenantEmailDomain
        {
            Id = Guid.NewGuid(),
            TenantId = dto.TenantId,
            Domain = domain,
            DefaultRoleName = role,
            AutoApprove = dto.AutoApprove,
            CreatedAtUtc = DateTime.UtcNow,
            IsDeleted = false
        };

        await _repository.AddAsync(entity, ct);
        await _repository.SaveChangesAsync(ct);

        return Map(entity);
    }

    public async Task<TenantEmailDomainResponseDTO?> UpdateAsync(Guid id, UpdateTenantEmailDomainDTO dto, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null)
            return null;

        var domain = NormalizeDomain(dto.Domain);
        var role = NormalizeRole(dto.DefaultRoleName);

        var duplicate = await _repository.GetByDomainAsync(domain, ct);
        if (duplicate is not null && duplicate.Id != entity.Id)
            throw new InvalidOperationException("This email domain is already used by another tenant.");

        entity.Domain = domain;
        entity.DefaultRoleName = role;
        entity.AutoApprove = dto.AutoApprove;
        entity.IsDeleted = false;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _repository.UpdateAsync(entity, ct);
        await _repository.SaveChangesAsync(ct);

        return Map(entity);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null)
            return false;

        await _repository.DeleteAsync(entity, ct);
        await _repository.SaveChangesAsync(ct);

        return true;
    }

    private static string NormalizeDomain(string domain)
    {
        if (string.IsNullOrWhiteSpace(domain))
            throw new InvalidOperationException("Domain is required.");

        var normalized = domain.Trim().ToLower();

        if (normalized.Contains("@"))
            normalized = normalized.Split('@').Last();

        normalized = normalized
            .Replace("https://", "")
            .Replace("http://", "")
            .Replace("www.", "")
            .Split('/')[0]
            .Trim();

        if (string.IsNullOrWhiteSpace(normalized) ||
            !normalized.Contains('.') ||
            normalized.Contains(' '))
        {
            throw new InvalidOperationException("Enter a valid domain, for example alphaevents.test.");
        }

        return normalized;
    }

    private static string NormalizeRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
            throw new InvalidOperationException("Default role is required.");

        var normalized = role.Trim();
        if (!AllowedRoles.Contains(normalized))
            throw new InvalidOperationException("Default role must be Staff, Admin, or TenantAdmin.");

        return normalized;
    }

    private static TenantEmailDomainResponseDTO Map(TenantEmailDomain entity)
    {
        return new TenantEmailDomainResponseDTO
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            Domain = entity.Domain,
            DefaultRoleName = entity.DefaultRoleName,
            AutoApprove = entity.AutoApprove,
            IsDeleted = entity.IsDeleted
        };
    }
}
