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
        if (
            httpContext.Request.Path.StartsWithSegments("/swagger") ||
            //httpContext.Request.Path.StartsWithSegments("/api/auth") ||
            httpContext.Request.Path.StartsWithSegments("/api/tenants") ||
            httpContext.Request.Path.StartsWithSegments("/api/health") ||
            httpContext.Request.Path.StartsWithSegments("/hangfire")
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

        var tenant = await tenantResolver.ResolveAsync(slug);

        if (tenant is null)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await _next(httpContext);
            return;
        }

        tenantContext.TenantId = tenant.Id;
        tenantContext.SchemaName = tenant.SchemaName;

        await _next(httpContext);
    }
}
