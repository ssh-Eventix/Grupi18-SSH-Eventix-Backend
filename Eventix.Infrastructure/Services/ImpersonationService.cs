using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Services;

public class ImpersonationService : IImpersonationService
{
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

    public async Task<(string Token, DateTime ExpiresAtUtc, Guid SessionId)> StartImpersonationAsync(Guid impersonatorTenantUserId, Guid targetTenantUserId, int minutes, string? reason = null, CancellationToken cancellationToken = default)
    {
        var target = await _userRepository.GetByIdAsync(targetTenantUserId, cancellationToken);
        if (target == null || !target.IsActive)
            throw new InvalidOperationException("Target user does not exist or is inactive.");

        var impersonator = await _userRepository.GetByIdAsync(impersonatorTenantUserId, cancellationToken);
        Guid? impersonatorPublicUserId = impersonator?.PublicUserId;

        var now = DateTime.UtcNow;
        var session = new TenantImpersonationLog
        {
            TenantId = _tenantContext.TenantId,
            ImpersonatorPublicUserId = impersonatorPublicUserId,
            ImpersonatorTenantUserId = impersonator?.Id,
            TargetTenantUserId = target.Id,
            StartedAtUtc = now,
            ExpiresAtUtc = now.AddMinutes(minutes),
            IsActive = true,
            Reason = reason
        };

        await _publicDb.TenantImpersonationLogs.AddAsync(session, cancellationToken);
        await _publicDb.SaveChangesAsync(cancellationToken);
        
        var roles = await _userRoleRepository.GetRoleNamesByUserIdAsync(target.Id, cancellationToken);

        var result = await _jwtTokenService.GenerateTokenAsync(
            subjectId: target.Id,
            email: target.Email,
            tenantId: _tenantContext.TenantId,
            roles: roles,
            isImpersonation: true,
            impersonationSessionId: session.Id,
            impersonatorPublicUserId: impersonatorPublicUserId,
            cancellationToken: cancellationToken);

        var token = result.Token;
        var expires = result.ExpiresAtUtc;

        return (token, expires, session.Id);
    }

    public async Task StopImpersonationAsync(Guid sessionId, Guid? actorTenantUserId = null, Guid? actorPublicUserId = null, CancellationToken cancellationToken = default)
    {
        var session = await _publicDb.TenantImpersonationLogs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == sessionId, cancellationToken);
        if (session == null)
            throw new InvalidOperationException("Impersonation session not found");

        var revocation = new TenantImpersonationEvent
        {
            SessionId = sessionId,
            EventType = ImpersonationEventType.Revoked,
            ActorPublicUserId = actorPublicUserId,
            ActorTenantUserId = actorTenantUserId,
            OccurredAtUtc = DateTime.UtcNow,
            Reason = "Revoked via API"
        };

        await _publicDb.TenantImpersonationEvents.AddAsync(revocation, cancellationToken);
        await _publicDb.SaveChangesAsync(cancellationToken);
    }
}


