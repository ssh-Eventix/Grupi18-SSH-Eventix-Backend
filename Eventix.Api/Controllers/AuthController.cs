using Eventix.Application.DTOs.User;
using Eventix.Domain.Entities;
using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserRoleEnum = Eventix.Domain.Enums.UserRole;

namespace Eventix.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPublicUserRepository _publicUserRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITenantContext _tenantContext;

    public AuthController(
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository,
        IRoleRepository roleRepository,
        IPublicUserRepository publicUserRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IRefreshTokenService refreshTokenService,
        IRefreshTokenRepository refreshTokenRepository,
        ITenantContext tenantContext)
    {
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
        _roleRepository = roleRepository;
        _publicUserRepository = publicUserRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _refreshTokenService = refreshTokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _tenantContext = tenantContext;
    }

    [HttpPost("register")]
    public async Task<ActionResult<LoginResponseDTO>> Register(
        [FromBody] RegisterRequestDTO dto,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) ||
            string.IsNullOrWhiteSpace(dto.Password) ||
            string.IsNullOrWhiteSpace(dto.FirstName) ||
            string.IsNullOrWhiteSpace(dto.LastName))
        {
            return BadRequest("FirstName, LastName, Email and Password are required.");
        }

        var existing = await _userRepository.GetByEmailAsync(dto.Email, ct);
        if (existing is not null && !existing.IsDeleted)
            return Conflict("A user with this email already exists.");

        var user = new User
        {
            TenantId = _tenantContext.TenantId,
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = dto.Email.Trim(),
            PasswordHash = _passwordHasher.Hash(dto.Password),
            IsActive = true
        };

        await _userRepository.AddAsync(user, ct);
        await _userRepository.SaveChangesAsync(ct);

        var roles = await _roleRepository.GetAllAsync(ct);
        var buyerRole = roles.FirstOrDefault(r =>
            string.Equals(r.Name, UserRoleEnum.Buyer.ToString(), StringComparison.OrdinalIgnoreCase) && !r.IsDeleted);

        if (buyerRole is null)
            return StatusCode(500, "Default Buyer role is not configured for this tenant.");

        await _userRoleRepository.AddAsync(new UserRole
        {
            TenantId = _tenantContext.TenantId,
            UserId = user.Id,
            RoleId = buyerRole.Id
        }, ct);
        await _userRoleRepository.SaveChangesAsync(ct);

        var mergedRoles = new List<string> { UserRoleEnum.Buyer.ToString() };

        var (accessToken, accessExpires) = await _jwtTokenService.GenerateTokenAsync(
            user.Id,
            user.Email,
            _tenantContext.TenantId,
            mergedRoles,
            cancellationToken: ct);
        var (refreshToken, refreshExpires) = await _refreshTokenService.CreateAsync(user.Id, ct);

        return Ok(new LoginResponseDTO
        {
            AccessToken = accessToken,
            AccessTokenExpiresAtUtc = accessExpires,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAtUtc = refreshExpires
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDTO>> Login(
        [FromBody] LoginRequestDTO dto,
        CancellationToken ct)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email, ct);
        PublicUser? publicUser = null;
        List<string> tenantRoles = new();

        if (user != null && !user.IsDeleted && user.IsActive)
        {
            if (!_passwordHasher.Verify(user.PasswordHash, dto.Password))
                return Unauthorized();

            tenantRoles = await _userRoleRepository.GetRoleNamesByUserIdAsync(user.Id, ct);

            if (user.PublicUserId.HasValue)
            {
                publicUser = await _publicUserRepository.GetByIdAsync(user.PublicUserId.Value, ct);
                if (publicUser != null && publicUser.IsActive)
                {
                    
                }
            }
        }
        else
        {
            publicUser = await _publicUserRepository.GetByEmailAsync(dto.Email, ct);
            if (publicUser is null || !publicUser.IsActive)
                return Unauthorized();

            if (!_passwordHasher.Verify(publicUser.PasswordHash, dto.Password))
                return Unauthorized();

            if (!publicUser.IsSuperAdmin)
                return Unauthorized();

            user = new User
            {
                TenantId = _tenantContext.TenantId,
                Email = publicUser.Email,
                FirstName = string.Empty,
                LastName = string.Empty,
                PasswordHash = _passwordHasher.Hash(Guid.NewGuid().ToString()),
                PublicUserId = publicUser.Id,
                IsActive = true
            };

            await _userRepository.AddAsync(user, ct);
            await _userRepository.SaveChangesAsync(ct);
        }

        var tenantRolesNormalized = tenantRoles
            .Where(r => !string.IsNullOrWhiteSpace(r))
            .Select(r => r.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        var mergedRoles = tenantRolesNormalized;

        var isSuperAdmin = publicUser != null && publicUser.IsSuperAdmin;

        var (accessToken, accessExpires) = await _jwtTokenService.GenerateTokenAsync(
            user.Id,
            user.Email,
            _tenantContext.TenantId,
            mergedRoles,
            isSuperAdmin,
            cancellationToken: ct);
        var (refreshToken, refreshExpires) = await _refreshTokenService.CreateAsync(user.Id, ct);

        return Ok(new LoginResponseDTO
        {
            AccessToken = accessToken,
            AccessTokenExpiresAtUtc = accessExpires,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAtUtc = refreshExpires
        });
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<LoginResponseDTO>> Refresh(
        [FromBody] RefreshRequestDTO dto,
        CancellationToken ct)
    {
        var hash = _refreshTokenService.Hash(dto.RefreshToken);

        var existing = await _refreshTokenRepository.GetByTokenHashAsync(hash, ct);

        if (existing is null || existing.IsExpired || existing.IsRevoked)
            return Unauthorized();

        var user = await _userRepository.GetByIdAsync(existing.UserId, ct);
        if (user is null)
            return Unauthorized();

        existing.RevokedAtUtc = DateTime.UtcNow;

        var tenantRoles = await _userRoleRepository.GetRoleNamesByUserIdAsync(user.Id, ct);
        if (user.PublicUserId.HasValue)
        {
            var publicUser = await _publicUserRepository.GetByIdAsync(user.PublicUserId.Value, ct);
            if (publicUser != null && publicUser.IsActive)
            {
                // simplified model: no GlobalUserRole mappings; use PublicUser.IsSuperAdmin instead
            }
        }

        var tenantRolesNormalized = tenantRoles
            .Where(r => !string.IsNullOrWhiteSpace(r))
            .Select(r => r.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        var mergedRoles = tenantRolesNormalized;

        var isSuperAdminRefresh = false;
        if (user.PublicUserId.HasValue)
        {
            var publicUser = await _publicUserRepository.GetByIdAsync(user.PublicUserId.Value, ct);
            if (publicUser != null && publicUser.IsActive)
            {
                isSuperAdminRefresh = publicUser.IsSuperAdmin;
            }
        }

        var (accessToken, accessExpires) = await _jwtTokenService.GenerateTokenAsync(
            user.Id,
            user.Email,
            _tenantContext.TenantId,
            mergedRoles,
            isSuperAdminRefresh,
            cancellationToken: ct);

        var (newRefreshToken, refreshExpires) =
            await _refreshTokenService.CreateAsync(user.Id, ct);

        existing.ReplacedByTokenHash = _refreshTokenService.Hash(newRefreshToken);

        await _refreshTokenRepository.UpdateAsync(existing, ct);

        return Ok(new LoginResponseDTO
        {
            AccessToken = accessToken,
            AccessTokenExpiresAtUtc = accessExpires,
            RefreshToken = newRefreshToken,
            RefreshTokenExpiresAtUtc = refreshExpires
        });
    }
}
