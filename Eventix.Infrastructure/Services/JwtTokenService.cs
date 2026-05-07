using Eventix.Application.Interfaces.Services;
using Eventix.Infrastructure.Auth;
using Eventix.Domain.Enums;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Eventix.Infrastructure.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _settings;
    private readonly byte[] _keyBytes;
    private readonly JwtSecurityTokenHandler _tokenHandler;

    public JwtTokenService(IOptions<JwtSettings> options)
    {
        _settings = options.Value;

        if (string.IsNullOrWhiteSpace(_settings.SecretKey))
            throw new InvalidOperationException("JWT SecretKey is not configured.");

        _keyBytes = Encoding.UTF8.GetBytes(_settings.SecretKey);
        _tokenHandler = new JwtSecurityTokenHandler();
    }

    public Task<(string Token, DateTime ExpiresAtUtc)> GenerateTokenAsync(
        Guid subjectId,
        string email,
        Guid tenantId,
        IEnumerable<string> roles,
        bool isSuperAdmin = false,
        Guid? impersonationSessionId = null,
        Guid? impersonatorPublicUserId = null,
        bool isImpersonation = false,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var expires = now.AddMinutes(_settings.ExpirationMinutes);

        var claims = BuildClaims(subjectId, email, tenantId, roles, isSuperAdmin, impersonationSessionId, impersonatorPublicUserId, isImpersonation);

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(_keyBytes),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            notBefore: now,
            expires: expires,
            signingCredentials: credentials);

        var jwt = _tokenHandler.WriteToken(token);

        return Task.FromResult((jwt, expires));
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var validationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false, // important
            ValidIssuer = _settings.Issuer,
            ValidAudience = _settings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(_keyBytes)
        };

        try
        {
            var principal = _tokenHandler.ValidateToken(token, validationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwt ||
                !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return principal;
        }
        catch
        {
            return null;
        }
    }

    private static List<Claim> BuildClaims(
        Guid subjectId,
        string email,
        Guid tenantId,
        IEnumerable<string> roles,
        bool isSuperAdmin,
        Guid? impersonationSessionId,
        Guid? impersonatorPublicUserId,
        bool isImpersonation)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, subjectId.ToString()),
            new Claim(ClaimTypes.NameIdentifier, subjectId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email ?? string.Empty),
            new Claim("tenantId", tenantId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var roleSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (roles != null)
        {
            foreach (var role in roles.Where(r => !string.IsNullOrWhiteSpace(r)))
            {
                roleSet.Add(role.Trim());
            }
        }

        if (isSuperAdmin)
            roleSet.Add(UserRole.SuperAdmin.ToString());

        foreach (var role in roleSet)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
            claims.Add(new Claim("role", role));
        }
        
        if (isSuperAdmin)
        {
            claims.Add(new Claim("isSuperAdmin", "true"));
        }
        
        if (isImpersonation && impersonationSessionId.HasValue)
        {
            claims.Add(new Claim("isImpersonation", "true"));
            claims.Add(new Claim("impersonationSessionId", impersonationSessionId.Value.ToString()));
            if (impersonatorPublicUserId.HasValue)
                claims.Add(new Claim("impersonatorPublicUserId", impersonatorPublicUserId.Value.ToString()));
        }

        return claims;
    }
}
