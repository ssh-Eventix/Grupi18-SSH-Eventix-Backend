using Eventix.Application.DTOs.Auth;
using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IPublicUserRepository _publicUserRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITenantContext _tenantContext;
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
        _publicUserRepository = publicUserRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _refreshTokenService = refreshTokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _tenantContext = tenantContext;
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

        var email = dto.Email.Trim().ToLower();

        var existingPublicUser = await _publicUserRepository.GetByEmailAsync(email, ct);

        if (existingPublicUser is not null && !existingPublicUser.IsDeleted)
            return Conflict("A user with this email already exists.");

        var publicUser = new PublicUser
        {
            Email = email,
            FullName = $"{dto.FirstName.Trim()} {dto.LastName.Trim()}",
            PasswordHash = _passwordHasher.Hash(dto.Password),
            IsActive = true
        };

        await _publicUserRepository.AddAsync(publicUser, ct);
        await _publicUserRepository.SaveChangesAsync(ct);

        var buyerRole = await _publicDb.PublicRoles
    .FirstOrDefaultAsync(x =>
        x.NormalizedName == "BUYER" &&
        !x.IsDeleted,
        ct);

        if (buyerRole is null)
        {
            buyerRole = new PublicRole
            {
                Name = "Buyer",
                NormalizedName = "BUYER",
                Description = "Public buyer role",
                IsDeleted = false
            };

            _publicDb.PublicRoles.Add(buyerRole);
            await _publicDb.SaveChangesAsync(ct);
        }

        _publicDb.PublicUserRoles.Add(new PublicUserRole
        {
            PublicUserId = publicUser.Id,
            PublicRoleId = buyerRole.Id
        });

        await _publicDb.SaveChangesAsync(ct);

        var (accessToken, accessExpires) = await _jwtTokenService.GenerateTokenAsync(
            subjectId: publicUser.Id,
            email: publicUser.Email,
            tenantId: Guid.Empty,
            roles: new List<string> { "Buyer" },
            cancellationToken: ct);

        var (refreshToken, refreshExpires) = await _refreshTokenService.CreateAsync(
            publicUser.Id,
            ct);

        return Ok(new LoginResponseDTO
        {
            AccessToken = accessToken,
            AccessTokenExpiresAtUtc = accessExpires,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAtUtc = refreshExpires,
            Role = "Buyer",
            TenantSlug = null
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
            var tenantUser = await _tenantDb.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.Email.ToLower() == email &&
                    x.TenantId == _tenantContext.TenantId &&
                    !x.IsDeleted &&
                    x.IsActive,
                    ct);

            if (tenantUser is null)
                return Unauthorized("Tenant user not found.");

            if (!_passwordHasher.Verify(tenantUser.PasswordHash, dto.Password))
                return Unauthorized("Invalid password.");

            var tenantRoles = await _tenantDb.UserRoles
                .AsNoTracking()
                .Include(x => x.Role)
                .Where(x =>
                    x.UserId == tenantUser.Id &&
                    x.TenantId == _tenantContext.TenantId &&
                    !x.IsDeleted &&
                    !x.Role.IsDeleted)
                .Select(x => x.Role.Name)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToListAsync(ct);

            if (!tenantRoles.Any())
                return Unauthorized("User has no roles in this tenant.");

            if (tenantUser.PublicUserId is null)
                return StatusCode(500, "Tenant user is not linked to PublicUser.");

            var (accessToken, accessExpires) = await _jwtTokenService.GenerateTokenAsync(
                subjectId: tenantUser.Id,
                email: tenantUser.Email,
                tenantId: _tenantContext.TenantId,
                roles: tenantRoles,
                cancellationToken: ct);

            var (refreshToken, refreshExpires) = await _refreshTokenService.CreateAsync(
                tenantUser.PublicUserId.Value,
                ct);

            return Ok(new LoginResponseDTO
            {
                AccessToken = accessToken,
                AccessTokenExpiresAtUtc = accessExpires,
                RefreshToken = refreshToken,
                RefreshTokenExpiresAtUtc = refreshExpires,
                Role = tenantRoles.FirstOrDefault(),
                TenantSlug = Request.Headers["X-Tenant-Slug"].FirstOrDefault()
            });
        }

        var publicUser = await _publicUserRepository.GetByEmailAsync(email, ct);

        if (publicUser is null || publicUser.IsDeleted || !publicUser.IsActive)
            return Unauthorized();

        if (!_passwordHasher.Verify(publicUser.PasswordHash, dto.Password))
            return Unauthorized();

        var publicRoles = await _publicDb.PublicUserRoles
            .AsNoTracking()
            .Include(x => x.PublicRole)
            .Where(x =>
                x.PublicUserId == publicUser.Id &&
                !x.PublicRole.IsDeleted)
            .Select(x => x.PublicRole.Name)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .ToListAsync(ct);

        if (publicRoles.Any(x =>
            string.Equals(x, "SuperAdmin", StringComparison.OrdinalIgnoreCase)))
        {
            var (superToken, superExpires) = await _jwtTokenService.GenerateTokenAsync(
                subjectId: publicUser.Id,
                email: publicUser.Email,
                tenantId: Guid.Empty,
                roles: publicRoles,
                isSuperAdmin: true,
                cancellationToken: ct);

            var (superRefresh, superRefreshExpires) = await _refreshTokenService.CreateAsync(
                publicUser.Id,
                ct);

            publicUser.LastLoginAtUtc = DateTime.UtcNow;
            await _publicUserRepository.UpdateAsync(publicUser, ct);
            await _publicUserRepository.SaveChangesAsync(ct);

            return Ok(new LoginResponseDTO
            {
                AccessToken = superToken,
                AccessTokenExpiresAtUtc = superExpires,
                RefreshToken = superRefresh,
                RefreshTokenExpiresAtUtc = superRefreshExpires,
                Role = "SuperAdmin",
                TenantSlug = null
            });
        }

        if (publicRoles.Any(x =>
            string.Equals(x, "Buyer", StringComparison.OrdinalIgnoreCase)))
        {
            var (buyerToken, buyerExpires) = await _jwtTokenService.GenerateTokenAsync(
                subjectId: publicUser.Id,
                email: publicUser.Email,
                tenantId: Guid.Empty,
                roles: new List<string> { "Buyer" },
                cancellationToken: ct);

            var (buyerRefresh, buyerRefreshExpires) = await _refreshTokenService.CreateAsync(
                publicUser.Id,
                ct);

            publicUser.LastLoginAtUtc = DateTime.UtcNow;
            await _publicUserRepository.UpdateAsync(publicUser, ct);
            await _publicUserRepository.SaveChangesAsync(ct);

            return Ok(new LoginResponseDTO
            {
                AccessToken = buyerToken,
                AccessTokenExpiresAtUtc = buyerExpires,
                RefreshToken = buyerRefresh,
                RefreshTokenExpiresAtUtc = buyerRefreshExpires,
                Role = "Buyer",
                TenantSlug = null
            });
        }

        return Ok(new LoginResponseDTO
        {
            TenantSlugRequired = true,
            Message = "Tenant slug is required for tenant users."
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

        existing.RevokedAtUtc = DateTime.UtcNow;

        var publicRoles = await _publicDb.PublicUserRoles
            .AsNoTracking()
            .Include(x => x.PublicRole)
            .Where(x =>
                x.PublicUserId == publicUser.Id &&
                !x.PublicRole.IsDeleted)
            .Select(x => x.PublicRole.Name)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .ToListAsync(ct);

        if (publicRoles.Any(x =>
            string.Equals(x, "SuperAdmin", StringComparison.OrdinalIgnoreCase)))
        {
            var (accessToken, accessExpires) = await _jwtTokenService.GenerateTokenAsync(
                subjectId: publicUser.Id,
                email: publicUser.Email,
                tenantId: Guid.Empty,
                roles: publicRoles,
                isSuperAdmin: true,
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
                RefreshTokenExpiresAtUtc = refreshExpires,
                Role = "SuperAdmin"
            });
        }

        if (publicRoles.Any(x =>
            string.Equals(x, "Buyer", StringComparison.OrdinalIgnoreCase)))
        {
            var (accessToken, accessExpires) = await _jwtTokenService.GenerateTokenAsync(
                subjectId: publicUser.Id,
                email: publicUser.Email,
                tenantId: Guid.Empty,
                roles: new List<string> { "Buyer" },
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
                RefreshTokenExpiresAtUtc = refreshExpires,
                Role = "Buyer"
            });
        }

        var tenantUser = await _userRepository.GetByPublicUserIdAsync(publicUser.Id, ct);

        if (tenantUser is null || tenantUser.IsDeleted || !tenantUser.IsActive)
            return Unauthorized();

        var tenantRoles = await _tenantDb.UserRoles
            .AsNoTracking()
            .Include(x => x.Role)
            .Where(x =>
                x.UserId == tenantUser.Id &&
                x.TenantId == _tenantContext.TenantId &&
                !x.IsDeleted &&
                !x.Role.IsDeleted)
            .Select(x => x.Role.Name)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .ToListAsync(ct);

        if (!tenantRoles.Any())
            return Unauthorized("User has no roles in this tenant.");

        var (tenantAccessToken, tenantAccessExpires) = await _jwtTokenService.GenerateTokenAsync(
            subjectId: tenantUser.Id,
            email: tenantUser.Email,
            tenantId: _tenantContext.TenantId,
            roles: tenantRoles,
            cancellationToken: ct);

        var (tenantNewRefreshToken, tenantRefreshExpires) = await _refreshTokenService.CreateAsync(
            publicUser.Id,
            ct);

        existing.ReplacedByTokenHash = _refreshTokenService.Hash(tenantNewRefreshToken);

        await _refreshTokenRepository.UpdateAsync(existing, ct);
        await _refreshTokenRepository.SaveChangesAsync(ct);

        return Ok(new LoginResponseDTO
        {
            AccessToken = tenantAccessToken,
            AccessTokenExpiresAtUtc = tenantAccessExpires,
            RefreshToken = tenantNewRefreshToken,
            RefreshTokenExpiresAtUtc = tenantRefreshExpires,
            Role = tenantRoles.FirstOrDefault()
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

        var token = await _refreshTokenRepository.GetByTokenHashAsync(hash, ct);

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

        var token = await _refreshTokenRepository.GetByTokenHashAsync(hash, ct);

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
    public async Task<IActionResult> GetRefreshTokens(CancellationToken ct)
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
        var tokens = await _refreshTokenRepository.GetByPublicUserIdAsync(publicUserId, ct);

        foreach (var token in tokens.Where(x => !x.IsRevoked))
        {
            token.RevokedAtUtc = DateTime.UtcNow;
            await _refreshTokenRepository.UpdateAsync(token, ct);
        }

        await _refreshTokenRepository.SaveChangesAsync(ct);

        return Ok("All refresh tokens revoked.");
    }
}