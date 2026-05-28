using Eventix.Application.Interfaces.Common;
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

        if (string.IsNullOrWhiteSpace(slug))
        {
            await _next(httpContext);
            return;
        }

        var tenant = await tenantResolver.ResolveAsync(slug.Trim());

        if (tenant is null)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await httpContext.Response.WriteAsync("Invalid tenant slug.");
            return;
        }

        tenantContext.TenantId = tenant.Id;
        tenantContext.SchemaName = tenant.SchemaName;

        await _next(httpContext);
    }
}