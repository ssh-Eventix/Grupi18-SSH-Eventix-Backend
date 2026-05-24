using Eventix.Application.DTOs.TenantEmailDomains;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventix.Infrastructure.Services
{
    public class TenantEmailDomainService : ITenantEmailDomainService
    {
        private readonly ITenantEmailDomainRepository _repository;
        private readonly ITenantRepository _tenantRepository;

        private static readonly HashSet<string> AllowedRoles = new(StringComparer.OrdinalIgnoreCase)
{
    "Buyer",
    "Staff",
    "Admin",
    "TenantAdmin"
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

            ValidateRole(dto.DefaultRoleName);

            var existing = await _repository.GetByDomainAsync(domain, ct);

            if (existing is not null)
                throw new InvalidOperationException("This email domain is already registered.");

            var entity = new TenantEmailDomain
            {
                Id = Guid.NewGuid(),
                TenantId = dto.TenantId,
                Domain = domain,
                DefaultRoleName = dto.DefaultRoleName.Trim(),
                AutoApprove = dto.AutoApprove,
                CreatedAtUtc = DateTime.UtcNow
            };

            await _repository.AddAsync(entity, ct);
            await _repository.SaveChangesAsync(ct);

            return Map(entity);
        }

        public async Task<TenantEmailDomainResponseDTO?> UpdateAsync(
            Guid id,
            UpdateTenantEmailDomainDTO dto,
            CancellationToken ct)
        {
            var entity = await _repository.GetByIdAsync(id, ct);

            if (entity is null)
                return null;

            var domain = NormalizeDomain(dto.Domain);

            ValidateRole(dto.DefaultRoleName);

            entity.Domain = domain;
            entity.DefaultRoleName = dto.DefaultRoleName.Trim();
            entity.AutoApprove = dto.AutoApprove;
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

            return domain.Trim().ToLower();
        }

        private static void ValidateRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role) || !AllowedRoles.Contains(role.Trim()))
                throw new InvalidOperationException("Default role must be Buyer, Staff, or Admin.");
        }

        private static TenantEmailDomainResponseDTO Map(TenantEmailDomain entity)
        {
            return new TenantEmailDomainResponseDTO
            {
                Id = entity.Id,
                TenantId = entity.TenantId,
                Domain = entity.Domain,
                DefaultRoleName = entity.DefaultRoleName,
                AutoApprove = entity.AutoApprove
            };
        }
    }
}
