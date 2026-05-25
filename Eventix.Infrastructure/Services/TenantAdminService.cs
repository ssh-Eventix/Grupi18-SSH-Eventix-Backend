using Eventix.Application.DTOs.TenantAdmins;
using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Eventix.Infrastructure.Services;

public class TenantAdminService : ITenantAdminService
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IPublicUserRepository _publicUserRepository;
    private readonly ITenantEmailDomainRepository _tenantEmailDomainRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IServiceScopeFactory _scopeFactory;

    private static readonly string[] AdminDomainRoles = { "Admin", "Staff" };

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
        Validate(dto);

        var tenant = await _tenantRepository.GetByIdAsync(dto.TenantId, ct);
        if (tenant is null)
            throw new InvalidOperationException("Tenant not found.");

        var email = dto.Email.Trim().ToLower();
        var emailDomain = GetEmailDomain(email);

        var tenantDomain = await _tenantEmailDomainRepository
            .GetByTenantIdAndDomainAsync(dto.TenantId, emailDomain, ct);

        if (tenantDomain is null)
            throw new InvalidOperationException($"Email domain '{emailDomain}' is not active or allowed for this tenant.");

        if (!AdminDomainRoles.Contains(tenantDomain.DefaultRoleName, StringComparer.OrdinalIgnoreCase))
            throw new InvalidOperationException("This domain is not configured for tenant admin users.");

        var existingPublicUser = await _publicUserRepository.GetByEmailAsync(email, ct);
        if (existingPublicUser is not null && !existingPublicUser.IsDeleted)
            throw new InvalidOperationException("A user with this email already exists.");

        var passwordHash = _passwordHasher.Hash(dto.Password);
        var now = DateTime.UtcNow;

        var publicUser = new PublicUser
        {
            Id = Guid.NewGuid(),
            Email = email,
            FullName = $"{dto.FirstName.Trim()} {dto.LastName.Trim()}",
            PasswordHash = passwordHash,
            IsActive = true,
            CreatedAtUtc = now,
            IsDeleted = false
        };

        await using var scope = _scopeFactory.CreateAsyncScope();

        var tenantContext = scope.ServiceProvider.GetRequiredService<ITenantContext>();
        tenantContext.TenantId = tenant.Id;
        tenantContext.SchemaName = tenant.SchemaName;

        var tenantDb = scope.ServiceProvider.GetRequiredService<TenantDbContext>();

        var roleName = tenantDomain.DefaultRoleName;

        var adminRole = await tenantDb.Roles
            .FirstOrDefaultAsync(x =>
                x.TenantId == tenant.Id &&
                x.Name == roleName &&
                !x.IsDeleted,
                ct);

        if (adminRole is null)
            throw new InvalidOperationException($"{roleName} role was not found for this tenant.");

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
            CreatedAtUtc = now,
            IsDeleted = false
        };

        await _publicUserRepository.AddAsync(publicUser, ct);
        await _publicUserRepository.SaveChangesAsync(ct);

        await tenantDb.Users.AddAsync(tenantUser, ct);
        await tenantDb.UserRoles.AddAsync(new UserRole
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.Id,
            UserId = tenantUser.Id,
            RoleId = adminRole.Id,
            AssignedAt = now,
            CreatedAtUtc = now,
            IsDeleted = false
        }, ct);

        await tenantDb.SaveChangesAsync(ct);

        return new TenantAdminResponseDTO
        {
            Id = tenantUser.Id,
            TenantId = tenant.Id,
            FirstName = tenantUser.FirstName,
            LastName = tenantUser.LastName,
            Email = tenantUser.Email,
            Role = adminRole.Name
        };
    }

    private static void Validate(CreateTenantAdminDTO dto)
    {
        if (dto.TenantId == Guid.Empty)
            throw new InvalidOperationException("Tenant is required.");

        if (string.IsNullOrWhiteSpace(dto.FirstName) ||
            string.IsNullOrWhiteSpace(dto.LastName) ||
            string.IsNullOrWhiteSpace(dto.Email) ||
            string.IsNullOrWhiteSpace(dto.Password))
            throw new InvalidOperationException("First name, last name, email and password are required.");
    }

    private static string GetEmailDomain(string email)
    {
        var parts = email.Split('@', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
            throw new InvalidOperationException("Invalid email address.");

        return parts[1].Trim().ToLower();
    }
}
