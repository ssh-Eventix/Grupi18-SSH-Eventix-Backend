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
             path.StartsWith("/api/auth")))
        {
            await _next(httpContext);
            return;
        }

        var slug = httpContext.Request.Headers["X-Tenant-Slug"].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(slug))
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await httpContext.Response.WriteAsync("Missing X-Tenant-Slug header.");
            return;
        }

        var tenant = await tenantResolver.ResolveAsync(slug);

        if (tenant is null)
        {
            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            await httpContext.Response.WriteAsync("Tenant not found.");
            return;
        }

        tenantContext.TenantId = tenant.Id;
        tenantContext.SchemaName = tenant.SchemaName;

        await _next(httpContext);
    }
}