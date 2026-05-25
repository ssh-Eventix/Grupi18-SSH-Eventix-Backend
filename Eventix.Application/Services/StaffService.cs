using Eventix.Application.DTOs.Staff;
using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventix.Application.Services
{
    public class StaffService : IStaffService
    {
        private const string StaffRoleName = "Staff";

        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IPublicUserRepository _publicUserRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITenantContext _tenantContext;

        public StaffService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IUserRoleRepository userRoleRepository,
            IPublicUserRepository publicUserRepository,
            IPasswordHasher passwordHasher,
            ITenantContext tenantContext)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _publicUserRepository = publicUserRepository;
            _passwordHasher = passwordHasher;
            _tenantContext = tenantContext;
        }

        public async Task<List<StaffResponseDTO>> GetAllAsync(CancellationToken ct = default)
        {
            var users = await _userRepository.GetAllAsync(ct);
            var result = new List<StaffResponseDTO>();

            foreach (var user in users.Where(x => !x.IsDeleted))
            {
                var roles = await _userRoleRepository.GetRoleNamesByUserIdAsync(user.Id, ct);

                if (roles.Any(x => string.Equals(x, StaffRoleName, StringComparison.OrdinalIgnoreCase)))
                    result.Add(Map(user));
            }

            return result;
        }

        public async Task<StaffResponseDTO> CreateAsync(CreateStaffDTO dto, CancellationToken ct = default)
        {
            var email = dto.Email.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(dto.FirstName))
                throw new InvalidOperationException("First name is required.");

            if (string.IsNullOrWhiteSpace(dto.LastName))
                throw new InvalidOperationException("Last name is required.");

            if (string.IsNullOrWhiteSpace(email))
                throw new InvalidOperationException("Email is required.");

            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new InvalidOperationException("Password is required.");

            if (_tenantContext.TenantId == Guid.Empty)
                throw new InvalidOperationException("Tenant context is missing.");

            var existingTenantUser = await _userRepository.GetByEmailAsync(email, ct);
            if (existingTenantUser is not null && !existingTenantUser.IsDeleted)
                throw new InvalidOperationException("A tenant user with this email already exists.");

            var existingPublicUser = await _publicUserRepository.GetByEmailAsync(email, ct);
            if (existingPublicUser is not null && !existingPublicUser.IsDeleted)
                throw new InvalidOperationException("A public user with this email already exists.");

            var staffRole = await GetOrCreateStaffRoleAsync(ct);
            var passwordHash = _passwordHasher.Hash(dto.Password);

            var publicUser = new PublicUser
            {
                Id = Guid.NewGuid(),
                Email = email,
                FullName = $"{dto.FirstName.Trim()} {dto.LastName.Trim()}",
                PasswordHash = passwordHash,
                IsActive = dto.IsActive,
                CreatedAtUtc = DateTime.UtcNow
            };

            await _publicUserRepository.AddAsync(publicUser, ct);
            await _publicUserRepository.SaveChangesAsync(ct);

            var tenantUser = new User
            {
                Id = Guid.NewGuid(),
                TenantId = _tenantContext.TenantId,
                PublicUserId = publicUser.Id,
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                Email = email,
                PasswordHash = passwordHash,
                IsActive = dto.IsActive,
                CreatedAtUtc = DateTime.UtcNow
            };

            await _userRepository.AddAsync(tenantUser, ct);

            await _userRoleRepository.AddAsync(new UserRole
            {
                Id = Guid.NewGuid(),
                TenantId = _tenantContext.TenantId,
                UserId = tenantUser.Id,
                RoleId = staffRole.Id,
                AssignedAt = DateTime.UtcNow,
                CreatedAtUtc = DateTime.UtcNow
            }, ct);

            await _userRoleRepository.SaveChangesAsync(ct);

            return Map(tenantUser);
        }

        public async Task<bool> DeactivateAsync(Guid id, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByIdAsync(id, ct);

            if (user is null || user.IsDeleted)
                return false;

            var roles = await _userRoleRepository.GetRoleNamesByUserIdAsync(user.Id, ct);
            var isStaff = roles.Any(x => string.Equals(x, StaffRoleName, StringComparison.OrdinalIgnoreCase));

            if (!isStaff)
                return false;

            user.IsActive = false;
            user.UpdatedAtUtc = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync(ct);

            return true;
        }

        private async Task<Role> GetOrCreateStaffRoleAsync(CancellationToken ct)
        {
            var roles = await _roleRepository.GetAllAsync(ct);

            var existing = roles.FirstOrDefault(x =>
                !x.IsDeleted &&
                string.Equals(x.Name, StaffRoleName, StringComparison.OrdinalIgnoreCase));

            if (existing is not null)
                return existing;

            var role = new Role
            {
                Id = Guid.NewGuid(),
                TenantId = _tenantContext.TenantId,
                Name = StaffRoleName,
                Description = "Staff members who can check in attendees.",
                IsGlobal = false,
                CreatedAtUtc = DateTime.UtcNow
            };

            await _roleRepository.AddAsync(role, ct);
            await _roleRepository.SaveChangesAsync(ct);

            return role;
        }

        private static StaffResponseDTO Map(User user) => new()
        {
            Id = user.Id,
            PublicUserId = user.PublicUserId ?? Guid.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = StaffRoleName,
            IsActive = user.IsActive,
            CreatedAtUtc = user.CreatedAtUtc
        };
    }
}
