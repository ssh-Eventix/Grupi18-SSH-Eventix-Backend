using System.Security.Cryptography;
using System.Text;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;

namespace Eventix.Infrastructure.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly IRefreshTokenRepository _repo;

    private const int TokenSize = 64;
    private const int ExpirationDays = 7;

    public RefreshTokenService(IRefreshTokenRepository repo)
    {
        _repo = repo;
    }

    public async Task<(string Token, DateTime ExpiresAtUtc)> CreateAsync(
        Guid publicUserId,
        CancellationToken ct)
    {
        var rawToken = GenerateSecureToken();
        var hash = Hash(rawToken);

        var entity = new RefreshToken
        {
            PublicUserId = publicUserId,
            TokenHash = hash,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(ExpirationDays)
        };

        await _repo.AddAsync(entity, ct);
        await _repo.SaveChangesAsync(ct);

        return (rawToken, entity.ExpiresAtUtc);
    }

    public string Hash(string token)
    {
        using var sha256 = SHA256.Create();

        var bytes = sha256.ComputeHash(
            Encoding.UTF8.GetBytes(token));

        return Convert.ToBase64String(bytes);
    }

    private static string GenerateSecureToken()
    {
        var bytes = new byte[TokenSize];

        RandomNumberGenerator.Fill(bytes);

        return Convert.ToBase64String(bytes);
    }
}