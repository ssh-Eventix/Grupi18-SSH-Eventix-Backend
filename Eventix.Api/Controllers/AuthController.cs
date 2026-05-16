using Eventix.Application.DTOs.Auth;
using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;
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
    private readonly ITenantEmailDomainRepository _tenantEmailDomainRepository;

    public AuthController(
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository,
        IRoleRepository roleRepository,
        IPublicUserRepository publicUserRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IRefreshTokenService refreshTokenService,
        IRefreshTokenRepository refreshTokenRepository,
        ITenantContext tenantContext,
        ITenantEmailDomainRepository tenantEmailDomainRepository)
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
        _tenantEmailDomainRepository = tenantEmailDomainRepository;
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

        var email = dto.Email.Trim().ToLower();

        var existingPublicUser = await _publicUserRepository.GetByEmailAsync(email, ct);
        if (existingPublicUser is not null && !existingPublicUser.IsDeleted)
            return Conflict("A user with this email already exists.");

        var existingTenantUser = await _userRepository.GetByEmailAsync(email, ct);
        if (existingTenantUser is not null && !existingTenantUser.IsDeleted)
            return Conflict("A tenant user with this email already exists.");

        var publicUser = new PublicUser
        {
            Email = email,
            FullName = $"{dto.FirstName.Trim()} {dto.LastName.Trim()}",
            PasswordHash = _passwordHasher.Hash(dto.Password),
            IsActive = true
        };

        await _publicUserRepository.AddAsync(publicUser, ct);
        await _publicUserRepository.SaveChangesAsync(ct);

        var user = new User
        {
            TenantId = _tenantContext.TenantId,
            PublicUserId = publicUser.Id,
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = email,
            PasswordHash = publicUser.PasswordHash,
            IsActive = true
        };

        await _userRepository.AddAsync(user, ct);
        await _userRepository.SaveChangesAsync(ct);

        var roles = await _roleRepository.GetAllAsync(ct);

        var emailDomain = GetEmailDomain(email);

        var tenantDomain = await _tenantEmailDomainRepository
            .GetByTenantIdAndDomainAsync(_tenantContext.TenantId, emailDomain, ct);

        var defaultRoleName = tenantDomain?.DefaultRoleName ?? UserRoleEnum.Buyer.ToString();

        var defaultRole = roles.FirstOrDefault(r =>
            string.Equals(r.Name, defaultRoleName, StringComparison.OrdinalIgnoreCase) &&
            !r.IsDeleted);

        if (defaultRole is null)
            return StatusCode(500, $"Default role '{defaultRoleName}' is not configured for this tenant.");

        await _userRoleRepository.AddAsync(new UserRole
        {
            TenantId = _tenantContext.TenantId,
            UserId = user.Id,
            RoleId = defaultRole.Id
        }, ct);

        await _userRoleRepository.SaveChangesAsync(ct);

        var tenantRoles = new List<string>
        {
            defaultRole.Name
        };

        var (accessToken, accessExpires) = await _jwtTokenService.GenerateTokenAsync(
            subjectId: user.Id,
            email: user.Email,
            tenantId: _tenantContext.TenantId,
            roles: tenantRoles,
            cancellationToken: ct);

        var (refreshToken, refreshExpires) = await _refreshTokenService.CreateAsync(
            publicUser.Id,
            ct);

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
        if (string.IsNullOrWhiteSpace(dto.Email) ||
            string.IsNullOrWhiteSpace(dto.Password))
        {
            return BadRequest("Email and password are required.");
        }

        var email = dto.Email.Trim().ToLower();

        var publicUser = await _publicUserRepository.GetByEmailAsync(email, ct);

        if (publicUser is null || publicUser.IsDeleted || !publicUser.IsActive)
            return Unauthorized();

        if (!_passwordHasher.Verify(publicUser.PasswordHash, dto.Password))
            return Unauthorized();

        var user = await _userRepository.GetByPublicUserIdAsync(publicUser.Id, ct);

        if (user is null || user.IsDeleted || !user.IsActive)
            return Unauthorized("User does not belong to this tenant.");

        var tenantRoles = await _userRoleRepository.GetRoleNamesByUserIdAsync(user.Id, ct);

        var roles = tenantRoles
            .Where(r => !string.IsNullOrWhiteSpace(r))
            .Select(r => r.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var (accessToken, accessExpires) = await _jwtTokenService.GenerateTokenAsync(
            subjectId: user.Id,
            email: user.Email,
            tenantId: _tenantContext.TenantId,
            roles: roles,
            cancellationToken: ct);

        var (refreshToken, refreshExpires) = await _refreshTokenService.CreateAsync(
            publicUser.Id,
            ct);

        publicUser.LastLoginAtUtc = DateTime.UtcNow;
        await _publicUserRepository.UpdateAsync(publicUser, ct);
        await _publicUserRepository.SaveChangesAsync(ct);

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
        if (string.IsNullOrWhiteSpace(dto.RefreshToken))
            return BadRequest("Refresh token is required.");

        var hash = _refreshTokenService.Hash(dto.RefreshToken);

        var existing = await _refreshTokenRepository.GetByTokenHashAsync(hash, ct);

        if (existing is null || existing.IsExpired || existing.IsRevoked)
            return Unauthorized();

        var publicUser = await _publicUserRepository.GetByIdAsync(existing.PublicUserId, ct);

        if (publicUser is null || publicUser.IsDeleted || !publicUser.IsActive)
            return Unauthorized();

        var user = await _userRepository.GetByPublicUserIdAsync(publicUser.Id, ct);

        if (user is null || user.IsDeleted || !user.IsActive)
            return Unauthorized();

        existing.RevokedAtUtc = DateTime.UtcNow;

        var tenantRoles = await _userRoleRepository.GetRoleNamesByUserIdAsync(user.Id, ct);

        var roles = tenantRoles
            .Where(r => !string.IsNullOrWhiteSpace(r))
            .Select(r => r.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var (accessToken, accessExpires) = await _jwtTokenService.GenerateTokenAsync(
            subjectId: user.Id,
            email: user.Email,
            tenantId: _tenantContext.TenantId,
            roles: roles,
            cancellationToken: ct);

        var (newRefreshToken, refreshExpires) = await _refreshTokenService.CreateAsync(
            publicUser.Id,
            ct);

        existing.ReplacedByTokenHash = _refreshTokenService.Hash(newRefreshToken);

        await _refreshTokenRepository.UpdateAsync(existing, ct);
        await _refreshTokenRepository.SaveChangesAsync(ct);

        return Ok(new LoginResponseDTO
        {
            AccessToken = accessToken,
            AccessTokenExpiresAtUtc = accessExpires,
            RefreshToken = newRefreshToken,
            RefreshTokenExpiresAtUtc = refreshExpires
        });
    }

    private static string GetEmailDomain(string email)
    {
        var parts = email.Split('@', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 2)
            throw new InvalidOperationException("Invalid email address.");

        return parts[1].Trim().ToLower();
    }
}