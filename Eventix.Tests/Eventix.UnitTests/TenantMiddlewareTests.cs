//using Eventix.Domain.Entities;
//using Microsoft.AspNetCore.Http;
//using Eventix.Infrastructure.MultiTenancy;
//using Eventix.Application.Interfaces.Common;
//using Moq;
//using Xunit;

//namespace Eventix.UnitTests;

//public class TenantMiddlewareTests
//{
//    [Fact]
//    public async Task Middleware_Should_Set_TenantContext_When_Header_Is_Valid()
//    {
//        var tenantId = Guid.NewGuid();

//        var httpContext = new DefaultHttpContext();
//        httpContext.Request.Path = "/api/events";
//        httpContext.Request.Headers["X-Tenant-Slug"] = "alpha-events";

//        var tenantContext = new TenantContext();

//        var resolver = new Mock<ITenantResolver>();
//        resolver.Setup(x => x.ResolveAsync("alpha-events", default))
//            .ReturnsAsync(new Tenant
//            {
//                Id = tenantId,
//                Slug = "alpha-events",
//                SchemaName = "tenant_alpha_events",
//                IsActive = true
//            });

//        var middleware = new TenantMiddleware(_ => Task.CompletedTask);

//        await middleware.InvokeAsync(httpContext, resolver.Object, tenantContext);

//        Assert.Equal(tenantId, tenantContext.TenantId);
//        Assert.Equal("tenant_alpha_events", tenantContext.SchemaName);
//    }

//    [Fact]
//    public async Task Middleware_Should_Skip_Tenant_Resolution_For_Tenants_Endpoint()
//    {
//        var httpContext = new DefaultHttpContext();
//        httpContext.Request.Path = "/api/tenants";

//        var tenantContext = new TenantContext();

//        var resolver = new Mock<ITenantResolver>();

//        var middleware = new TenantMiddleware(_ => Task.CompletedTask);

//        await middleware.InvokeAsync(httpContext, resolver.Object, tenantContext);

//        resolver.Verify(x => x.ResolveAsync(It.IsAny<string>(), default), Times.Never);
//    }
//}