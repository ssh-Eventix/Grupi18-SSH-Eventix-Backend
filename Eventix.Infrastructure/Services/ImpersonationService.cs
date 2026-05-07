using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Services;

public class ImpersonationService : IImpersonationService
{
    private const int MaxImpersonationDurationMinutes = 120;
    private const int MaxReasonLength = 500;

    private readonly PublicDbContext _publicDb;
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ITenantContext _tenantContext;

    public ImpersonationService(
        PublicDbContext publicDb,
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository,
        IJwtTokenService jwtTokenService,
        ITenantContext tenantContext)
    {
        _publicDb = publicDb;
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
        _jwtTokenService = jwtTokenService;
        _tenantContext = tenantContext;
    }

    public async Task<ImpersonationStartResult> StartImpersonationAsync(
        Guid impersonatorTenantUserId,
        Guid targetTenantUserId,
        int minutes,
        string? reason = null,
        CancellationToken cancellationToken = default)
    {
        if (minutes <= 0 || minutes > MaxImpersonationDurationMinutes)
            throw new ArgumentOutOfRangeException(
                nameof(minutes),
                $"Impersonation duration must be between 1 and {MaxImpersonationDurationMinutes} minutes.");

        var normalizedReason = string.IsNullOrWhiteSpace(reason)
            ? null
            : reason.Trim();

        if (normalizedReason is { Length: > MaxReasonLength })
            throw new ArgumentOutOfRangeException(
                nameof(reason),
                $"Impersonation reason cannot exceed {MaxReasonLength} characters.");

        var impersonator = await _userRepository.GetByIdAsync(
            impersonatorTenantUserId,
            cancellationToken);

        if (impersonator == null || !impersonator.IsActive)
            throw new InvalidOperationException(
                "Impersonator does not exist or is inactive.");

        var target = await _userRepository.GetByIdAsync(
            targetTenantUserId,
            cancellationToken);

        if (target == null || !target.IsActive)
            throw new InvalidOperationException(
                "Target user does not exist or is inactive.");

        if (impersonatorTenantUserId == targetTenantUserId)
            throw new InvalidOperationException(
                "Users cannot impersonate themselves.");

        var now = DateTime.UtcNow;

        var hasActiveSession = await _publicDb.TenantImpersonationLogs
            .AnyAsync(x =>
                    x.TenantId == _tenantContext.TenantId &&
                    x.TargetTenantUserId == targetTenantUserId &&
                    x.IsActive &&
                    x.ExpiresAtUtc > now,
                cancellationToken);

        if (hasActiveSession)
            throw new InvalidOperationException(
                "Target user already has an active impersonation session.");

        var session = new TenantImpersonationLog
        {
            TenantId = _tenantContext.TenantId,
            ImpersonatorPublicUserId = impersonator.PublicUserId,
            ImpersonatorTenantUserId = impersonator.Id,
            TargetTenantUserId = target.Id,
            StartedAtUtc = now,
            ExpiresAtUtc = now.AddMinutes(minutes),
            IsActive = true,
            Reason = normalizedReason
        };

        await _publicDb.TenantImpersonationLogs
            .AddAsync(session, cancellationToken);

        await _publicDb.SaveChangesAsync(cancellationToken);

        var roles = await _userRoleRepository
            .GetRoleNamesByUserIdAsync(
                target.Id,
                cancellationToken);

        var tokenResult = await _jwtTokenService.GenerateTokenAsync(
            subjectId: target.Id,
            email: target.Email,
            tenantId: _tenantContext.TenantId,
            roles: roles,
            isImpersonation: true,
            impersonationSessionId: session.Id,
            impersonatorPublicUserId: impersonator.PublicUserId,
            cancellationToken: cancellationToken);

        return new ImpersonationStartResult(
            tokenResult.Token,
            tokenResult.ExpiresAtUtc,
            session.Id);
    }

    public async Task StopImpersonationAsync(
        Guid sessionId,
        CancellationToken cancellationToken = default)
    {
        var session = await _publicDb.TenantImpersonationLogs
            .FirstOrDefaultAsync(
                x => x.Id == sessionId,
                cancellationToken);

        if (session == null)
            throw new InvalidOperationException(
                "Impersonation session not found.");

        if (!session.IsActive)
            return;

        var now = DateTime.UtcNow;

        session.IsActive = false;
        session.RevokedAtUtc = now;
        session.ExpiresAtUtc = now;

        await _publicDb.SaveChangesAsync(cancellationToken);
    }
}
