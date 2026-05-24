using Eventix.Application.DTOs.Auth;
using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Infrastructure.Persistence.Database;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserRoleEnum = Eventix.Domain.Enums.UserRole;

namespace Eventix.Api.Controllers;

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
    private readonly PublicDbContext _publicDb;
    private readonly TenantDbContext _tenantDb;

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
        ITenantEmailDomainRepository tenantEmailDomainRepository,
        PublicDbContext publicDb,
        TenantDbContext tenantDb)
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
        _publicDb = publicDb;
        _tenantDb = tenantDb;
    }

    [HttpPost("register")]
    [AllowAnonymous]
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

        if (_tenantContext.TenantId == Guid.Empty || string.IsNullOrWhiteSpace(_tenantContext.SchemaName))
        {
            return BadRequest("Tenant context is missing.");
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
    [AllowAnonymous]
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

        if (_tenantContext.TenantId != Guid.Empty &&
            !string.IsNullOrWhiteSpace(_tenantContext.SchemaName))
        {
            var user = await _tenantDb.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.Email.ToLower() == email &&
                    x.TenantId == _tenantContext.TenantId &&
                    !x.IsDeleted &&
                    x.IsActive,
                    ct);

            if (user is null)
                return Unauthorized("Tenant user not found.");

            if (!_passwordHasher.Verify(user.PasswordHash, dto.Password))
                return Unauthorized("Invalid password.");

            var roles = await _tenantDb.UserRoles
                .AsNoTracking()
                .Include(x => x.Role)
                .Where(x =>
                    x.UserId == user.Id &&
                    x.TenantId == _tenantContext.TenantId &&
                    !x.IsDeleted &&
                    !x.Role.IsDeleted)
                .Select(x => x.Role.Name)
                .Distinct()
                .ToListAsync(ct);

            if (!roles.Any())
                return Unauthorized("User has no roles in this tenant.");

            var (accessToken, accessExpires) =
                await _jwtTokenService.GenerateTokenAsync(
                    subjectId: user.Id,
                    email: user.Email,
                    tenantId: _tenantContext.TenantId,
                    roles: roles,
                    cancellationToken: ct);

            if (user.PublicUserId is null)
                return StatusCode(500, "Tenant user is not linked to PublicUser.");

            var (refreshToken, refreshExpires) =
                await _refreshTokenService.CreateAsync(user.PublicUserId.Value, ct);

            return Ok(new LoginResponseDTO
            {
                AccessToken = accessToken,
                AccessTokenExpiresAtUtc = accessExpires,
                RefreshToken = refreshToken,
                RefreshTokenExpiresAtUtc = refreshExpires
            });
        }

        var publicUser = await _publicUserRepository.GetByEmailAsync(email, ct);

        if (publicUser is null || publicUser.IsDeleted || !publicUser.IsActive)
            return Unauthorized();

        if (!_passwordHasher.Verify(publicUser.PasswordHash, dto.Password))
            return Unauthorized();

        var publicRoles = await _publicDb.PublicUserRoles
            .AsNoTracking()
            .Where(x => x.PublicUserId == publicUser.Id)
            .Select(x => x.PublicRole.Name)
            .ToListAsync(ct);

        if (!publicRoles.Any(x =>
            string.Equals(x, "SuperAdmin", StringComparison.OrdinalIgnoreCase)))
        {
            return BadRequest("Tenant slug is required for tenant users.");
        }

        var (superToken, superExpires) =
            await _jwtTokenService.GenerateTokenAsync(
                subjectId: publicUser.Id,
                email: publicUser.Email,
                tenantId: Guid.Empty,
                roles: publicRoles,
                isSuperAdmin: true,
                cancellationToken: ct);

        var (superRefresh, superRefreshExpires) =
            await _refreshTokenService.CreateAsync(publicUser.Id, ct);

        publicUser.LastLoginAtUtc = DateTime.UtcNow;
        await _publicUserRepository.UpdateAsync(publicUser, ct);
        await _publicUserRepository.SaveChangesAsync(ct);

        return Ok(new LoginResponseDTO
        {
            AccessToken = superToken,
            AccessTokenExpiresAtUtc = superExpires,
            RefreshToken = superRefresh,
            RefreshTokenExpiresAtUtc = superRefreshExpires
        });
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
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

        var roles = await _tenantDb.UserRoles
            .AsNoTracking()
            .Include(x => x.Role)
            .Where(x =>
                x.UserId == user.Id &&
                x.TenantId == _tenantContext.TenantId &&
                !x.IsDeleted &&
                !x.Role.IsDeleted)
            .Select(x => x.Role.Name)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .ToListAsync(ct);

        if (!roles.Any())
            return Unauthorized("User has no roles in this tenant.");

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

    [HttpPost("logout")]
    [Authorize(Policy = "Permission:RevokeRefreshTokens")]
    public async Task<IActionResult> Logout(
    [FromBody] RefreshRequestDTO dto,
    CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.RefreshToken))
            return BadRequest("Refresh token is required.");

        var hash = _refreshTokenService.Hash(dto.RefreshToken);

        var token = await _refreshTokenRepository
            .GetByTokenHashAsync(hash, ct);

        if (token is null)
            return NotFound();

        if (token.IsRevoked)
            return BadRequest("Token already revoked.");

        token.RevokedAtUtc = DateTime.UtcNow;

        await _refreshTokenRepository.UpdateAsync(token, ct);
        await _refreshTokenRepository.SaveChangesAsync(ct);

        return Ok("Logged out successfully.");
    }

    [HttpPost("revoke-refresh-token")]
    [Authorize(Policy = "Permission:RevokeRefreshTokens")]
    public async Task<IActionResult> RevokeRefreshToken(
    [FromBody] RefreshRequestDTO dto,
    CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.RefreshToken))
            return BadRequest("Refresh token is required.");

        var hash = _refreshTokenService.Hash(dto.RefreshToken);

        var token = await _refreshTokenRepository
            .GetByTokenHashAsync(hash, ct);

        if (token is null)
            return NotFound();

        if (token.IsRevoked)
            return BadRequest("Token already revoked.");

        token.RevokedAtUtc = DateTime.UtcNow;

        await _refreshTokenRepository.UpdateAsync(token, ct);
        await _refreshTokenRepository.SaveChangesAsync(ct);

        return Ok("Refresh token revoked.");
    }

    [HttpGet("refresh-tokens")]
    [Authorize(Policy = "Permission:ViewRefreshTokens")]
    public async Task<IActionResult> GetRefreshTokens(
    CancellationToken ct)
    {
        var tokens = await _refreshTokenRepository.GetAllAsync(ct);

        var result = tokens.Select(x => new
        {
            x.Id,
            x.PublicUserId,
            x.PublicUser.Email,
            x.CreatedAtUtc,
            x.ExpiresAtUtc,
            x.RevokedAtUtc,
            x.ReplacedByTokenHash,
            x.IsExpired,
            x.IsRevoked
        });

        return Ok(result);
    }

    [HttpPost("revoke-all/{publicUserId:guid}")]
    [Authorize(Policy = "Permission:RevokeRefreshTokens")]
    public async Task<IActionResult> RevokeAllUserTokens(
    Guid publicUserId,
    CancellationToken ct)
    {
        var tokens = await _refreshTokenRepository
            .GetByPublicUserIdAsync(publicUserId, ct);

        foreach (var token in tokens.Where(x => !x.IsRevoked))
        {
            token.RevokedAtUtc = DateTime.UtcNow;

            await _refreshTokenRepository.UpdateAsync(token, ct);
        }

        await _refreshTokenRepository.SaveChangesAsync(ct);

        return Ok("All refresh tokens revoked.");
    }

    private static string GetEmailDomain(string email)
    {
        var parts = email.Split('@', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 2)
            throw new InvalidOperationException("Invalid email address.");

        return parts[1].Trim().ToLower();
    }
}