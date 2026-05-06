using System;

namespace Eventix.Application.Interfaces.Services;

public interface IImpersonationService
{
    Task<(string Token, DateTime ExpiresAtUtc, Guid SessionId)> StartImpersonationAsync(Guid impersonatorTenantUserId, Guid targetTenantUserId, int minutes, string? reason = null, CancellationToken cancellationToken = default);
    Task StopImpersonationAsync(Guid sessionId, Guid? actorTenantUserId = null, Guid? actorPublicUserId = null, CancellationToken cancellationToken = default);
}

