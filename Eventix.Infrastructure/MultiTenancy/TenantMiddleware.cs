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
        var path = httpContext.Request.Path.Value?.ToLower();

        if (path != null &&
            (path.StartsWith("/swagger") ||
             path.StartsWith("/api/tenants") ||
             path.StartsWith("/api/auth")||
             path.StartsWith("/api/health")||
             path.StartsWith("/hangfire")))
        {
            await _next(httpContext);
            return;
        }

        var slug = httpContext.Request.Headers["X-Tenant-Slug"].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(slug))
        {
            await _next(httpContext);
            return;
        }

        var tenant = await tenantResolver.ResolveAsync(slug);

        if (tenant is null)
        {
            await _next(httpContext);
            return;
        }

        tenantContext.TenantId = tenant.Id;
        tenantContext.SchemaName = tenant.SchemaName;

        await _next(httpContext);
    }
}
