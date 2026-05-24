using Eventix.Application.DTOs.TenantAdmins;
using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventix.Infrastructure.Services
{
    public class TenantAdminService : ITenantAdminService
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IPublicUserRepository _publicUserRepository;
        private readonly ITenantEmailDomainRepository _tenantEmailDomainRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IServiceScopeFactory _scopeFactory;

        public TenantAdminService(
            ITenantRepository tenantRepository,
            IPublicUserRepository publicUserRepository,
            ITenantEmailDomainRepository tenantEmailDomainRepository,
            IPasswordHasher passwordHasher,
            IServiceScopeFactory scopeFactory)
        {
            _tenantRepository = tenantRepository;
            _publicUserRepository = publicUserRepository;
            _tenantEmailDomainRepository = tenantEmailDomainRepository;
            _passwordHasher = passwordHasher;
            _scopeFactory = scopeFactory;
        }

        public async Task<TenantAdminResponseDTO> CreateAsync(CreateTenantAdminDTO dto, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(dto.FirstName) ||
                string.IsNullOrWhiteSpace(dto.LastName) ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Password))
            {
                throw new InvalidOperationException("First name, last name, email and password are required.");
            }

            var tenant = await _tenantRepository.GetByIdAsync(dto.TenantId, ct);

            if (tenant is null)
                throw new InvalidOperationException("Tenant not found.");

            var email = dto.Email.Trim().ToLower();
            var emailDomain = GetEmailDomain(email);

            var tenantDomain = await _tenantEmailDomainRepository
                .GetByTenantIdAndDomainAsync(dto.TenantId, emailDomain, ct);

            if (tenantDomain is null)
                throw new InvalidOperationException("Email domain is not allowed for this tenant.");

            var allowedRoles = new[] { "Admin", "TenantAdmin" };

            if (!allowedRoles.Contains(tenantDomain.DefaultRoleName, StringComparer.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Email domain does not create tenant admin users.");
            }

            var existingPublicUser = await _publicUserRepository.GetByEmailAsync(email, ct);

            if (existingPublicUser is not null && !existingPublicUser.IsDeleted)
                throw new InvalidOperationException("A user with this email already exists.");

            var passwordHash = _passwordHasher.Hash(dto.Password);

            var publicUser = new PublicUser
            {
                Id = Guid.NewGuid(),
                Email = email,
                FullName = $"{dto.FirstName.Trim()} {dto.LastName.Trim()}",
                PasswordHash = passwordHash,
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow
            };

            await _publicUserRepository.AddAsync(publicUser, ct);
            await _publicUserRepository.SaveChangesAsync(ct);

            await using var scope = _scopeFactory.CreateAsyncScope();

            var tenantContext = scope.ServiceProvider.GetRequiredService<ITenantContext>();
            tenantContext.TenantId = tenant.Id;
            tenantContext.SchemaName = tenant.SchemaName;

            var tenantDb = scope.ServiceProvider.GetRequiredService<TenantDbContext>();

            var tenantUser = new User
            {
                Id = Guid.NewGuid(),
                TenantId = tenant.Id,
                PublicUserId = publicUser.Id,
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                Email = email,
                PasswordHash = passwordHash,
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow
            };

            await tenantDb.Users.AddAsync(tenantUser, ct);

            var adminRole = await tenantDb.Roles
                .FirstOrDefaultAsync(x =>
                    x.TenantId == tenant.Id &&
                    (x.Name == "Admin" || x.Name == "TenantAdmin") &&
                    !x.IsDeleted,
                    ct);

            if (adminRole is null)
                throw new InvalidOperationException("Admin role was not found for this tenant.");

            await tenantDb.UserRoles.AddAsync(new UserRole
            {
                Id = Guid.NewGuid(),
                TenantId = tenant.Id,
                UserId = tenantUser.Id,
                RoleId = adminRole.Id,
                AssignedAt = DateTime.UtcNow,
                CreatedAtUtc = DateTime.UtcNow
            }, ct);

            await tenantDb.SaveChangesAsync(ct);

            return new TenantAdminResponseDTO
            {
                Id = tenantUser.Id,
                TenantId = tenant.Id,
                FirstName = tenantUser.FirstName,
                LastName = tenantUser.LastName,
                Email = tenantUser.Email,
                Role = "Admin"
            };
        }

        private static string GetEmailDomain(string email)
        {
            var parts = email.Split('@', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
                throw new InvalidOperationException("Invalid email address.");

            return parts[1].Trim().ToLower();
        }
    }
}
