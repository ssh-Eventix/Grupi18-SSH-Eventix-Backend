using Eventix.Infrastructure.MultiTenancy;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Eventix.IntegrationTests.MultiTenancy;

public class TenantSchemaProvisionerTests
    : IClassFixture<TestDatabaseFixture>
{
    private readonly TestDatabaseFixture _fixture;

    public TenantSchemaProvisionerTests(
        TestDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ProvisionTenantSchemaAsync_Should_Create_Schema()
    {
        var schemaName =
            $"tenant_test_{Guid.NewGuid():N}";

        using var scope =
            _fixture.Services.CreateScope();

        var provisioner =
            scope.ServiceProvider
                .GetRequiredService<TenantSchemaProvisioner>();

        await provisioner
            .ProvisionTenantSchemaAsync(schemaName);

        await using var conn =
            new NpgsqlConnection(_fixture.ConnectionString);

        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            @"SELECT schema_name
              FROM information_schema.schemata
              WHERE schema_name = @schema",
            conn);

        cmd.Parameters.AddWithValue(
            "schema",
            schemaName);

        var result = await cmd.ExecuteScalarAsync();

        Assert.Equal(schemaName, result);
    }
}