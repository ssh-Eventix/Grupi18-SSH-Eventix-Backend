using Eventix.Application.DTOs.AuditLog;
using Eventix.Application.DTOs.Common;
using Eventix.Application.DTOs.Events;
using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;
using Eventix.Domain.Enums;
using Eventix.Infrastructure.MultiTenancy;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.Extensions.DependencyInjection;

namespace Eventix.IntegrationTests.Events;

public class EventServiceIntegrationTests : IClassFixture<TestDatabaseFixture>
{
    private readonly TestDatabaseFixture _fixture;

    public EventServiceIntegrationTests(TestDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreateAsync_Should_Create_Event_In_Tenant_Schema()
    {
        var tenantId = Guid.NewGuid();
        var schemaName = $"tenant_test_{Guid.NewGuid():N}";

        await using var cleanup = new SchemaCleanup(_fixture, schemaName);

        using var scope = _fixture.Services.CreateScope();

        var tenantContext = scope.ServiceProvider.GetRequiredService<ITenantContext>();
        tenantContext.TenantId = tenantId;
        tenantContext.SchemaName = schemaName;

        var provisioner = scope.ServiceProvider.GetRequiredService<TenantSchemaProvisioner>();
        await provisioner.ProvisionTenantSchemaAsync(schemaName);

        var db = scope.ServiceProvider.GetRequiredService<TenantDbContext>();

        var category = new EventCategory
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = "Conference",
            Description = "Tech conference",
            Icon = "calendar",
            DisplayOrder = 1,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        var venue = new Venue
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = "Main Hall",
            Code = "MAIN-HALL",
            AddressLine1 = "Rruga B",
            City = "Prishtina",
            Country = "Kosovo",
            TotalCapacity = 500,
            IsIndoor = true,
            IsAccessible = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        db.EventCategories.Add(category);
        db.Venues.Add(venue);
        await db.SaveChangesAsync();

        var service = scope.ServiceProvider.GetRequiredService<IEventService>();

        var dto = new CreateEventDTO
        {
            VenueId = venue.Id,
            EventCategoryId = category.Id,
            Title = "Eventix Launch",
            Slug = "eventix-launch",
            Description = "Launch event",
            OrganizerName = "Eventix Team",
            StartUtc = DateTime.UtcNow.AddDays(10),
            EndUtc = DateTime.UtcNow.AddDays(10).AddHours(2),
            Visibility = EventVisibility.Public,
            Currency = "EUR"
        };

        var result = await service.CreateAsync(dto);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Eventix Launch", result.Title);
        Assert.Equal("eventix-launch", result.Slug);
        Assert.Equal(tenantId, db.Events.First().TenantId);
    }

    [Fact]
    public async Task GetAllAsync_With_Search_Should_Return_Filtered_Events()
    {
        var tenantId = Guid.NewGuid();
        var schemaName = $"tenant_test_{Guid.NewGuid():N}";

        await using var cleanup = new SchemaCleanup(_fixture, schemaName);

        using var scope = _fixture.Services.CreateScope();

        var tenantContext = scope.ServiceProvider.GetRequiredService<ITenantContext>();
        tenantContext.TenantId = tenantId;
        tenantContext.SchemaName = schemaName;

        var provisioner = scope.ServiceProvider.GetRequiredService<TenantSchemaProvisioner>();
        await provisioner.ProvisionTenantSchemaAsync(schemaName);

        var db = scope.ServiceProvider.GetRequiredService<TenantDbContext>();

        var category = new EventCategory
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = "Music",
            IsActive = true,
            DisplayOrder = 1,
            CreatedAtUtc = DateTime.UtcNow
        };

        var venue = new Venue
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = "Arena",
            Code = "ARENA",
            AddressLine1 = "Rruga B",
            City = "Prishtina",
            Country = "Kosovo",
            TotalCapacity = 1000,
            IsIndoor = true,
            IsAccessible = true,
            CreatedAtUtc = DateTime.UtcNow
        };
        
        db.EventCategories.Add(category);
        db.Venues.Add(venue);

        db.Events.AddRange(
            new Eventix.Domain.Entities.Event
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                VenueId = venue.Id,
                EventCategoryId = category.Id,
                Title = "Rock Night",
                Slug = "rock-night",
                StartUtc = DateTime.UtcNow.AddDays(5),
                EndUtc = DateTime.UtcNow.AddDays(5).AddHours(2),
                Currency = "EUR",
                CreatedAtUtc = DateTime.UtcNow
            },
            new Eventix.Domain.Entities.Event
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                VenueId = venue.Id,
                EventCategoryId = category.Id,
                Title = "Business Summit",
                Slug = "business-summit",
                StartUtc = DateTime.UtcNow.AddDays(6),
                EndUtc = DateTime.UtcNow.AddDays(6).AddHours(2),
                Currency = "EUR",
                CreatedAtUtc = DateTime.UtcNow
            });

        await db.SaveChangesAsync();

        var service = scope.ServiceProvider.GetRequiredService<IEventService>();

        var result = await service.GetAllAsync("rock");

        Assert.Single(result);
        Assert.Equal("Rock Night", result[0].Title);
    }

    private sealed class SchemaCleanup : IAsyncDisposable
    {
        private readonly TestDatabaseFixture _fixture;
        private readonly string _schemaName;

        public SchemaCleanup(TestDatabaseFixture fixture, string schemaName)
        {
            _fixture = fixture;
            _schemaName = schemaName;
        }

        public async ValueTask DisposeAsync()
        {
            await _fixture.DropSchemaAsync(_schemaName);
        }
    }

    public class FakeCurrentUserService : ICurrentUserService
    {
        public Guid? UserId => Guid.NewGuid();

        public string? Email => "test@eventix.com";
    }

    public class FakeAuditLogService : IAuditLogService
    {
        public Task<PagedResult<AuditLogDTO>> GetPagedAsync(
            AuditLogQueryDTO query,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new PagedResult<AuditLogDTO>
            {
                Items = new List<AuditLogDTO>(),
                TotalCount = 0,
                Page = 1,
                PageSize = 10
            });
        }

        public Task<AuditLogDTO?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<AuditLogDTO?>(null);
        }

        public Task CreateAsync(
            CreateAuditLogDTO dto,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}