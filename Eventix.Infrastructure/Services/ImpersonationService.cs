using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;
using Eventix.Domain.Enums;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Eventix.Application.DTOs.Auth;

namespace Eventix.Infrastructure.Services;

public class ImpersonationService : IImpersonationService
{
    private const int MaxImpersonationDurationMinutes = 120;
    private const int MaxReasonLength = 500;

    private readonly PublicDbContext _publicDb;
    private readonly IPublicUserRepository _publicUserRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IJwtTokenService _jwtTokenService;

    public ImpersonationService(
        PublicDbContext publicDb,
        IPublicUserRepository publicUserRepository,
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository,
        IJwtTokenService jwtTokenService)
    {
        _publicDb = publicDb;
        _publicUserRepository = publicUserRepository;
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<ImpersonationStartResult> StartImpersonationAsync(
        Guid superAdminPublicUserId,
        Guid targetTenantId,
        Guid targetPublicUserId,
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

        var superAdmin = await _publicUserRepository.GetByIdAsync(
            superAdminPublicUserId,
            cancellationToken);

        if (superAdmin is null || superAdmin.IsDeleted || !superAdmin.IsActive)
            throw new InvalidOperationException("SuperAdmin user does not exist or is inactive.");

        var isSuperAdmin = superAdmin.PublicUserRoles.Any(x =>
            string.Equals(x.PublicRole.Name, "SuperAdmin", StringComparison.OrdinalIgnoreCase));

        if (!isSuperAdmin)
            throw new UnauthorizedAccessException("Only SuperAdmin can impersonate users.");

        var targetPublicUser = await _publicUserRepository.GetByIdAsync(
            targetPublicUserId,
            cancellationToken);

        if (targetPublicUser is null || targetPublicUser.IsDeleted || !targetPublicUser.IsActive)
            throw new InvalidOperationException("Target public user does not exist or is inactive.");

        if (superAdminPublicUserId == targetPublicUserId)
            throw new InvalidOperationException("SuperAdmin cannot impersonate himself.");

        var targetTenantUser = await _userRepository.GetByPublicUserIdAsync(
            targetPublicUserId,
            cancellationToken);

        if (targetTenantUser is null || targetTenantUser.IsDeleted || !targetTenantUser.IsActive)
            throw new InvalidOperationException("Target tenant user does not exist or is inactive.");

        var now = DateTime.UtcNow;

        var hasActiveSession = await _publicDb.TenantImpersonationLogs
            .AnyAsync(x =>
                x.SuperAdminUserId == superAdminPublicUserId &&
                x.TargetTenantId == targetTenantId &&
                x.TargetUserId == targetPublicUserId &&
                x.IsActive &&
                x.ExpiresAtUtc > now,
                cancellationToken);

        if (hasActiveSession)
            throw new InvalidOperationException("This impersonation session is already active.");

        var session = new TenantImpersonationLog
        {
            SuperAdminUserId = superAdminPublicUserId,
            TargetTenantId = targetTenantId,
            TargetUserId = targetPublicUserId,
            StartedAtUtc = now,
            ExpiresAtUtc = now.AddMinutes(minutes),
            IsActive = true,
            Reason = normalizedReason,
            Event = TenantImpersonationEvent.Started
        };

        await _publicDb.TenantImpersonationLogs.AddAsync(session, cancellationToken);
        await _publicDb.SaveChangesAsync(cancellationToken);

        var roles = await _userRoleRepository.GetRoleNamesByUserIdAsync(
            targetTenantUser.Id,
            cancellationToken);

        var tokenResult = await _jwtTokenService.GenerateTokenAsync(
            subjectId: targetTenantUser.Id,
            email: targetTenantUser.Email,
            tenantId: targetTenantId,
            roles: roles,
            isImpersonation: true,
            impersonationSessionId: session.Id,
            impersonatorPublicUserId: superAdminPublicUserId,
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
            .FirstOrDefaultAsync(x => x.Id == sessionId, cancellationToken);

        if (session is null)
            throw new InvalidOperationException("Impersonation session not found.");

        if (!session.IsActive)
            return;

        var now = DateTime.UtcNow;

        session.IsActive = false;
        session.RevokedAtUtc = now;
        session.ExpiresAtUtc = now;
        session.Event = TenantImpersonationEvent.Ended;
        session.UpdatedAtUtc = now;

        await _publicDb.SaveChangesAsync(cancellationToken);
    }
}