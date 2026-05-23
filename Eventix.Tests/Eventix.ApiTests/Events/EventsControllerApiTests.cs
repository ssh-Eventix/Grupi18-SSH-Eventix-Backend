using Eventix.Api.Controllers;
using Eventix.Application.DTOs.Events;
using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;
using Eventix.Application.Services;
using Eventix.Domain.Enums;
using Eventix.Infrastructure.MultiTenancy;
using Eventix.Infrastructure.Persistence.Database;
using Eventix.Infrastructure.Persistence.Repositories;
using Eventix.Tests.ApiTests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

namespace Eventix.ApiTests.Events;

public class EventsControllerApiTests
{
    [Fact]
    public async Task Create_Should_Return_CreatedAtAction()
    {
        var services = CreateServices();

        using var scope = services.CreateScope();

        var tenantContext = scope.ServiceProvider.GetRequiredService<ITenantContext>();
        tenantContext.TenantId = Guid.NewGuid();
        tenantContext.SchemaName = $"tenant_api_test_{Guid.NewGuid():N}";

        var provisioner = scope.ServiceProvider.GetRequiredService<TenantSchemaProvisioner>();
        await provisioner.ProvisionTenantSchemaAsync(tenantContext.SchemaName);

        var db = scope.ServiceProvider.GetRequiredService<TenantDbContext>();

        var category = new Eventix.Domain.Entities.EventCategory
        {
            Id = Guid.NewGuid(),
            TenantId = tenantContext.TenantId,
            Name = "Conference",
            IsActive = true,
            DisplayOrder = 1,
            CreatedAtUtc = DateTime.UtcNow
        };

        var venue = new Eventix.Domain.Entities.Venue
        {
            Id = Guid.NewGuid(),
            TenantId = tenantContext.TenantId,
            Name = "API Hall",
            Code = "API-HALL",
            AddressLine1 = "Rruga B",
            City = "Prishtina",
            Country = "Kosovo",
            TotalCapacity = 300,
            IsIndoor = true,
            IsAccessible = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        db.EventCategories.Add(category);
        db.Venues.Add(venue);
        await db.SaveChangesAsync();

        var controller = CreateController(scope);

        var dto = new CreateEventDTO
        {
            VenueId = venue.Id,
            EventCategoryId = category.Id,
            Title = "API Test Event",
            Slug = "api-test-event",
            StartUtc = DateTime.UtcNow.AddDays(3),
            EndUtc = DateTime.UtcNow.AddDays(3).AddHours(2),
            Visibility = EventVisibility.Public,
            Currency = "EUR"
        };

        var response = await controller.Create(dto, CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(response.Result);
        var value = Assert.IsType<EventResponseDTO>(created.Value);

        Assert.Equal("API Test Event", value.Title);
    }

    [Fact]
    public async Task GetById_When_Event_Does_Not_Exist_Should_Return_NotFound()
    {
        var services = CreateServices();

        using var scope = services.CreateScope();

        var tenantContext = scope.ServiceProvider.GetRequiredService<ITenantContext>();
        tenantContext.TenantId = Guid.NewGuid();
        tenantContext.SchemaName = $"tenant_api_test_{Guid.NewGuid():N}";

        var provisioner = scope.ServiceProvider.GetRequiredService<TenantSchemaProvisioner>();
        await provisioner.ProvisionTenantSchemaAsync(tenantContext.SchemaName);

        var controller = CreateController(scope);

        var response = await controller.GetById(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<NotFoundResult>(response.Result);
    }

    private static ServiceProvider CreateServices()
    {
        var connectionString =
       TestConfiguration.ConnectionString;

        var services = new ServiceCollection();

        services.AddScoped<ITenantContext, TenantContext>();
        services.AddScoped<TenantSchemaProvisioner>();

        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IEventService, EventService>();

        services.AddDistributedMemoryCache();

        services.AddDbContext<TenantDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString);
            options.ReplaceService<IModelCacheKeyFactory, TenantModelCacheKeyFactory>();
        });

        return services.BuildServiceProvider();
    }

    private static EventsController CreateController(IServiceScope scope)
    {
        return new EventsController(
            scope.ServiceProvider.GetRequiredService<IEventService>(),
            scope.ServiceProvider.GetRequiredService<IDistributedCache>(),
            scope.ServiceProvider.GetRequiredService<ITenantContext>());
    }
}