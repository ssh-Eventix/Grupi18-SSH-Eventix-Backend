namespace Eventix.Application.Interfaces.Services;

public sealed record ImpersonationStartResult(string Token, DateTime ExpiresAtUtc, Guid SessionId);

public interface IImpersonationService
{
    Task<ImpersonationStartResult> StartImpersonationAsync(
        Guid impersonatorTenantUserId,
        Guid targetTenantUserId,
        int minutes,
        string? reason = null,
        CancellationToken cancellationToken = default);

    Task StopImpersonationAsync(Guid sessionId, CancellationToken cancellationToken = default);
}