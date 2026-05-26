using Eventix.Application.DTOs.Events;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Enums;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;

namespace Eventix.Infrastructure.Persistence.Repositories;

public class PublicEventRepository : IPublicEventRepository
{
    private readonly PublicDbContext _publicDbContext;

    public PublicEventRepository(
        PublicDbContext publicDbContext)
    {
        _publicDbContext = publicDbContext;
    }

    public async Task<List<EventResponseDTO>> GetAllPublicAsync(
        string? search,
        CancellationToken cancellationToken = default)
    {
        var schemas = await GetActiveTenantSchemasAsync(cancellationToken);

        var result = new List<EventResponseDTO>();

        foreach (var schema in schemas)
        {
            try
            {
                var events = await GetEventsFromSchemaAsync(
                    schema,
                    search,
                    null,
                    cancellationToken);

                result.AddRange(events);
            }
            catch (PostgresException ex) when (ex.SqlState == "42P01" || ex.SqlState == "3F000")
            {
                continue;
            }
        }

        return result
            .OrderBy(x => x.StartUtc)
            .ToList();
    }

    public async Task<EventResponseDTO?> GetPublicByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var schemas = await GetActiveTenantSchemasAsync(cancellationToken);

        foreach (var schema in schemas)
        {
            try
            {
                var events = await GetEventsFromSchemaAsync(
                    schema,
                    null,
                    id,
                    cancellationToken);

                var item = events.FirstOrDefault();

                if (item is not null)
                    return item;
            }
            catch (PostgresException ex) when (ex.SqlState == "42P01" || ex.SqlState == "3F000")
            {
                continue;
            }
        }

        return null;
    }

    private async Task<List<string>> GetActiveTenantSchemasAsync(CancellationToken cancellationToken)
    {
        return await _publicDbContext.Tenants
            .AsNoTracking()
            .Where(x => x.IsActive && x.SchemaName != null && x.SchemaName != "")
            .Select(x => x.SchemaName!)
            .ToListAsync(cancellationToken);
    }

    private async Task<List<EventResponseDTO>> GetEventsFromSchemaAsync(
        string schema,
        string? search,
        Guid? id,
        CancellationToken cancellationToken)
    {
        var connection =
            (NpgsqlConnection)_publicDbContext.Database.GetDbConnection();

        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }
       

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
  AND e.""IsPublished"" = true
    AND (
        e.""Visibility""::text = @publicVisibilityText
        OR e.""Visibility""::text = @publicVisibilityNumber
    )
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

        command.Parameters.Add("id", NpgsqlDbType.Uuid).Value =
            id.HasValue ? id.Value : DBNull.Value;

        command.Parameters.Add("search", NpgsqlDbType.Text).Value =
            string.IsNullOrWhiteSpace(search) ? DBNull.Value : search.Trim();

        command.Parameters.Add("publicVisibilityText", NpgsqlDbType.Text).Value = "Public";
        command.Parameters.Add("publicVisibilityNumber", NpgsqlDbType.Text).Value =
            ((int)EventVisibility.Public).ToString();

        var result = new List<EventResponseDTO>();

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            result.Add(new EventResponseDTO
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

        return result;
    }

    private static string QuoteIdentifier(string value)
    {
        return "\"" + value.Replace("\"", "\"\"") + "\"";
    }
}