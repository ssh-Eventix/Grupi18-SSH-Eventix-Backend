using Eventix.Application.DTOs.Auth;
namespace Eventix.Application.Interfaces.Services;

public interface IImpersonationService
{
    Task<ImpersonationStartResult> StartImpersonationAsync(
        Guid superAdminPublicUserId,
        Guid targetTenantId,
        Guid targetPublicUserId,
        int minutes,
        string? reason = null,
        CancellationToken cancellationToken = default);

    Task StopImpersonationAsync(
        Guid sessionId,
        CancellationToken cancellationToken = default);
}