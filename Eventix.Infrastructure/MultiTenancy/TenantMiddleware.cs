using Eventix.Application.Interfaces.Common;
using Eventix.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Eventix.Infrastructure.MultiTenancy;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext httpContext,
        ITenantResolver tenantResolver,
        ITenantContext tenantContext)
    {
        var path = httpContext.Request.Path;

        if (
            path.StartsWithSegments("/swagger") ||
            path.StartsWithSegments("/hangfire") ||
            path.StartsWithSegments("/api/health") ||
            path.StartsWithSegments("/api/tenants") ||
            path.StartsWithSegments("/api/auth/register") ||
            path.StartsWithSegments("/api/auth/refresh") ||
            path.StartsWithSegments("/api/auth/logout") ||
            path.StartsWithSegments("/api/auth/revoke-refresh-token") ||
            path.StartsWithSegments("/api/auth/refresh-tokens") ||
            path.StartsWithSegments("/api/auth/revoke-all")||
            path.StartsWithSegments("/api/auditlog")
        )
        {
            await _next(httpContext);
            return;
        }

        var slug = httpContext.Request.Headers["X-Tenant-Slug"]
            .FirstOrDefault();

        var tenant = string.IsNullOrWhiteSpace(slug)
            ? await ResolveTenantFromTokenAsync(httpContext, tenantResolver)
            : await tenantResolver.ResolveAsync(slug.Trim());

        if (tenant is null)
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                await _next(httpContext);
                return;
            }

            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await httpContext.Response.WriteAsync("Invalid tenant slug.");
            return;
        }

        tenantContext.TenantId = tenant.Id;
        tenantContext.SchemaName = tenant.SchemaName;

        await _next(httpContext);
    }

    private static async Task<Tenant?> ResolveTenantFromTokenAsync(
        HttpContext httpContext,
        ITenantResolver tenantResolver)
    {
        var tenantIdValue = httpContext.User.FindFirst("tenantId")?.Value;

        if (!Guid.TryParse(tenantIdValue, out var tenantId) || tenantId == Guid.Empty)
        {
            return null;
        }

        return await tenantResolver.ResolveByIdAsync(tenantId);
    }
}
