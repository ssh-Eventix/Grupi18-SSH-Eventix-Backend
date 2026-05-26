using Eventix.Application.DTOs.Events;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Enums;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
namespace Eventix.Infrastructure.Services;

public class PublicEventService : IPublicEventService
{
    private readonly PublicDbContext _publicDbContext;

    public PublicEventService(PublicDbContext publicDbContext)
    {
        _publicDbContext = publicDbContext;
    }

    public async Task<List<EventResponseDTO>> GetAllPublicAsync(
        string? search,
        CancellationToken cancellationToken = default)
    {
        var tenants = await _publicDbContext.Tenants
            .AsNoTracking()
            .Where(t => t.IsActive && t.SchemaName != null && t.SchemaName != "")
            .Select(t => t.SchemaName)
            .ToListAsync(cancellationToken);

        var result = new List<EventResponseDTO>();

        foreach (var schema in tenants)
        {
            try
            {
                result.AddRange(await GetEventsFromTenantAsync(schema, search, null, cancellationToken));
            }
            catch (PostgresException ex) when (ex.SqlState == "42P01" || ex.SqlState == "3F000")
            {
                continue;
            }
        }

        return result
            .OrderBy(e => e.StartUtc)
            .ToList();
    }

    public async Task<EventResponseDTO?> GetPublicByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var tenants = await _publicDbContext.Tenants
            .AsNoTracking()
            .Where(t => t.IsActive && t.SchemaName != null && t.SchemaName != "")
            .Select(t => t.SchemaName)
            .ToListAsync(cancellationToken);

        foreach (var schema in tenants)
        {
            try
            {
                var result = await GetEventsFromTenantAsync(schema, null, id, cancellationToken);

                if (result.Count > 0)
                    return result[0];
            }
            catch (PostgresException ex) when (ex.SqlState == "42P01" || ex.SqlState == "3F000")
            {
                continue;
            }
        }

        return null;
    }

    private async Task<List<EventResponseDTO>> GetEventsFromTenantAsync(
        string schema,
        string? search,
        Guid? id,
        CancellationToken cancellationToken)
    {
        var connectionString = BuildConnectionStringFromEnvironment();

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        var quotedSchema = QuoteIdentifier(schema);

        var sql = $@"
SELECT
    e.""Id"",
    e.""VenueId"",
    v.""Name"" AS ""VenueName"",
    e.""EventCategoryId"",
    c.""Name"" AS ""EventCategoryName"",
    e.""Title"",
    e.""Slug"",
    e.""Description"",
    e.""OrganizerName"",
    e.""StartUtc"",
    e.""EndUtc"",
    e.""Status"",
    e.""Visibility"",
    e.""BannerImageUrl"",
    e.""MaxTicketsPerOrder"",
    e.""MinTicketsPerOrder"",
    e.""IsFree"",
    e.""IsPublished"",
    e.""Currency"",
    e.""CreatedAtUtc"",
    e.""UpdatedAtUtc""
FROM {quotedSchema}.""Event"" e
LEFT JOIN {quotedSchema}.""Venue"" v ON v.""Id"" = e.""VenueId""
LEFT JOIN {quotedSchema}.""EventCategory"" c ON c.""Id"" = e.""EventCategoryId""
WHERE e.""IsDeleted"" = false
  AND e.""EndUtc"" > NOW()
  AND (@id IS NULL OR e.""Id"" = @id)
  AND (
      @search IS NULL
      OR e.""Title"" ILIKE '%' || @search || '%'
      OR e.""Description"" ILIKE '%' || @search || '%'
      OR c.""Name"" ILIKE '%' || @search || '%'
      OR v.""Name"" ILIKE '%' || @search || '%'
  )
ORDER BY e.""StartUtc"";
";

        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.Add("id", NpgsqlTypes.NpgsqlDbType.Uuid).Value =
    id.HasValue ? id.Value : DBNull.Value;

        command.Parameters.Add("search", NpgsqlTypes.NpgsqlDbType.Text).Value =
            string.IsNullOrWhiteSpace(search) ? DBNull.Value : search;

        var events = new List<EventResponseDTO>();

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            events.Add(new EventResponseDTO
            {
                Id = reader.GetGuid(0),
                VenueId = reader.GetGuid(1),
                VenueName = reader.IsDBNull(2) ? null : reader.GetString(2),
                EventCategoryId = reader.GetGuid(3),
                EventCategoryName = reader.IsDBNull(4) ? null : reader.GetString(4),
                Title = reader.GetString(5),
                Slug = reader.GetString(6),
                Description = reader.IsDBNull(7) ? null : reader.GetString(7),
                OrganizerName = reader.IsDBNull(8) ? null : reader.GetString(8),
                StartUtc = reader.GetDateTime(9),
                EndUtc = reader.GetDateTime(10),
                Status = (EventStatus)reader.GetInt32(11),
                Visibility = (EventVisibility)reader.GetInt32(12),
                BannerImageUrl = reader.IsDBNull(13) ? null : reader.GetString(13),
                MaxTicketsPerOrder = reader.GetInt32(14),
                MinTicketsPerOrder = reader.GetInt32(15),
                IsFree = reader.GetBoolean(16),
                IsPublished = reader.GetBoolean(17),
                Currency = reader.GetString(18),
                CreatedAtUtc = reader.GetDateTime(19),
                UpdatedAtUtc = reader.IsDBNull(20) ? null : reader.GetDateTime(20)
            });
        }

        return events;
    }

    private static string BuildConnectionStringFromEnvironment()
    {
        var dbHost = Environment.GetEnvironmentVariable("POSTGRES_HOST");
        var dbPort = Environment.GetEnvironmentVariable("POSTGRES_PORT");
        var dbName = Environment.GetEnvironmentVariable("POSTGRES_DB");
        var dbUser = Environment.GetEnvironmentVariable("POSTGRES_USER");
        var dbPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");

        if (string.IsNullOrWhiteSpace(dbHost) ||
            string.IsNullOrWhiteSpace(dbPort) ||
            string.IsNullOrWhiteSpace(dbName) ||
            string.IsNullOrWhiteSpace(dbUser) ||
            string.IsNullOrWhiteSpace(dbPassword))
        {
            throw new InvalidOperationException("Database environment variables are missing.");
        }

        return
            $"Host={dbHost};" +
            $"Port={dbPort};" +
            $"Database={dbName};" +
            $"Username={dbUser};" +
            $"Password={dbPassword}";
    }

    private static string QuoteIdentifier(string value)
    {
        return "\"" + value.Replace("\"", "\"\"") + "\"";
    }
}