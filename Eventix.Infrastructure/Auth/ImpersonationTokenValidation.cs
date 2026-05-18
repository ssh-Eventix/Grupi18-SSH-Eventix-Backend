using System.Security.Claims;
using Eventix.Application.Interfaces.Common;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Eventix.Infrastructure.Auth
{
    public static class ImpersonationTokenValidation
    {
        public static async Task ValidateAsync(TokenValidatedContext context)
        {
            var principal = context.Principal;
            if (principal == null)
                return;

            var isImpersonation = principal.HasClaim(c =>
                c.Type == ImpersonationAuthConstants.IsImpersonationClaim &&
                c.Value == "true");
            if (!isImpersonation)
                return;

            var sessionClaim = principal.FindFirst(ImpersonationAuthConstants.ImpersonationSessionIdClaim)?.Value;
            if (!Guid.TryParse(sessionClaim, out var sessionId))
            {
                context.Fail("Invalid impersonation session claim");
                return;
            }

            var publicDb = context.HttpContext.RequestServices.GetRequiredService<PublicDbContext>();
            var tenantContext = context.HttpContext.RequestServices.GetRequiredService<ITenantContext>();

            var session = await publicDb.TenantImpersonationLogs
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == sessionId, context.HttpContext.RequestAborted);

            if (session == null)
            {
                context.Fail("Impersonation session not found");
                return;
            }

            if (session.TargetTenantId != tenantContext.TenantId)
            {
                context.Fail("Tenant mismatch");
                return;
            }

            if (!session.IsActive || session.RevokedAtUtc.HasValue)
            {
                context.Fail("Impersonation session revoked");
                return;
            }

            if (session.ExpiresAtUtc <= DateTime.UtcNow)
            {
                context.Fail("Impersonation session expired");
                return;
            }

            var subjectClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(subjectClaim, out var subjectUserId))
            {
                context.Fail("Invalid subject claim");
                return;
            }

        }
    }

}
