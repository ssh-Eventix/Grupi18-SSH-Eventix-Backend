using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using Eventix.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Eventix.IntegrationTests.TenantEmailDomains;

public class TenantEmailDomainRepositoryIntegrationTests
    : IClassFixture<TestDatabaseFixture>
{
    private readonly TestDatabaseFixture _fixture;

    public TenantEmailDomainRepositoryIntegrationTests(TestDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task AddAsync_Should_Save_Email_Domain()
    {
        await using var scope = _fixture.Services.CreateAsyncScope();

        var db = scope.ServiceProvider.GetRequiredService<PublicDbContext>();
        await EnsurePublicDatabaseReadyAsync(db);

        var tenant = await CreateTenantAsync(db);
        var repository = new TenantEmailDomainRepository(db);

        var entity = new TenantEmailDomain
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.Id,
            Domain = $"test-{Guid.NewGuid():N}.com",
            DefaultRoleName = "Buyer",
            AutoApprove = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        await repository.AddAsync(entity, CancellationToken.None);
        await repository.SaveChangesAsync(CancellationToken.None);

        var saved = await repository.GetByIdAsync(entity.Id, CancellationToken.None);

        Assert.NotNull(saved);
        Assert.Equal(entity.Domain, saved!.Domain);
        Assert.Equal("Buyer", saved.DefaultRoleName);
        Assert.True(saved.AutoApprove);
    }

    [Fact]
    public async Task GetByTenantIdAsync_Should_Return_Only_Tenant_Domains()
    {
        await using var scope = _fixture.Services.CreateAsyncScope();

        var db = scope.ServiceProvider.GetRequiredService<PublicDbContext>();
        await EnsurePublicDatabaseReadyAsync(db);

        var tenant1 = await CreateTenantAsync(db);
        var tenant2 = await CreateTenantAsync(db);

        db.TenantEmailDomains.AddRange(
            new TenantEmailDomain
            {
                Id = Guid.NewGuid(),
                TenantId = tenant1.Id,
                Domain = $"alpha-{Guid.NewGuid():N}.com",
                DefaultRoleName = "Buyer",
                AutoApprove = true,
                CreatedAtUtc = DateTime.UtcNow
            },
            new TenantEmailDomain
            {
                Id = Guid.NewGuid(),
                TenantId = tenant2.Id,
                Domain = $"beta-{Guid.NewGuid():N}.com",
                DefaultRoleName = "Staff",
                AutoApprove = false,
                CreatedAtUtc = DateTime.UtcNow
            });

        await db.SaveChangesAsync();

        var repository = new TenantEmailDomainRepository(db);

        var result = await repository.GetByTenantIdAsync(
            tenant1.Id,
            CancellationToken.None);

        Assert.Single(result);
        Assert.Equal(tenant1.Id, result[0].TenantId);
    }

    [Fact]
    public async Task DeleteAsync_Should_Soft_Delete_Domain()
    {
        await using var scope = _fixture.Services.CreateAsyncScope();

        var db = scope.ServiceProvider.GetRequiredService<PublicDbContext>();
        await EnsurePublicDatabaseReadyAsync(db);

        var tenant = await CreateTenantAsync(db);

        var entity = new TenantEmailDomain
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.Id,
            Domain = $"delete-{Guid.NewGuid():N}.com",
            DefaultRoleName = "Buyer",
            AutoApprove = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        db.TenantEmailDomains.Add(entity);
        await db.SaveChangesAsync();

        var repository = new TenantEmailDomainRepository(db);

        await repository.DeleteAsync(entity, CancellationToken.None);
        await repository.SaveChangesAsync(CancellationToken.None);

        var result = await repository.GetByIdAsync(entity.Id, CancellationToken.None);

        Assert.Null(result);
    }

    private static async Task<Tenant> CreateTenantAsync(PublicDbContext db)
    {
        var id = Guid.NewGuid();

        var tenant = new Tenant
        {
            Id = id,
            Name = $"Tenant {id:N}",
            Slug = $"tenant-{id:N}",
            SchemaName = $"tenant_{id:N}",
            ContactEmail = $"admin-{id:N}@test.com",
            City = "Prishtina",
            Country = "Kosovo",
            IsActive = true,
            IsTrial = false,
            CreatedAtUtc = DateTime.UtcNow
        };

        db.Tenants.Add(tenant);
        await db.SaveChangesAsync();

        return tenant;
    }

    private static async Task EnsurePublicDatabaseReadyAsync(PublicDbContext db)
    {
        await db.Database.ExecuteSqlRawAsync("""
        CREATE TABLE IF NOT EXISTS public."AuditLog" (
            "Id" uuid NOT NULL,
            "TenantId" uuid NULL,
            "TenantName" text NULL,
            "UserId" uuid NULL,
            "UserEmail" text NULL,
            "EntityName" text NOT NULL,
            "EntityId" uuid NULL,
            "Action" integer NOT NULL,
            "OldValues" text NULL,
            "NewValues" text NULL,
            "CreatedAtUtc" timestamp with time zone NOT NULL,
            CONSTRAINT "PK_AuditLog" PRIMARY KEY ("Id")
        );
    """);

        await db.Database.MigrateAsync();
    }
}