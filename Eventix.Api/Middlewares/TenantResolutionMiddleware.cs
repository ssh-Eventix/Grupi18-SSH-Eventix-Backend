using Eventix.Infrastructure.MultiTenancy;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.API.Middlewares;

public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext, PublicDbContext publicDbContext)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        if (path.StartsWith("/swagger") || path.StartsWith("/api/auth") || path.StartsWith("/api/tenants"))
        {
            await _next(context);
            return;
        }

        var tenantSlug = context.Request.Headers["X-Tenant-Slug"].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(tenantSlug))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("Missing X-Tenant-Slug header.");
            return;
        }

        var tenant = await publicDbContext.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Slug == tenantSlug && x.IsActive);

        if (tenant is null)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsync("Tenant not found.");
            return;
        }

        tenantContext.SetTenant(tenant.Id, tenant.Slug, tenant.SchemaName);

        await _next(context);
    }
}