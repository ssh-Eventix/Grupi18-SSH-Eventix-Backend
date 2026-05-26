using Eventix.Application.DTOs.Archive;
using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Services;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Eventix.Infrastructure.BackgroundJobs;

public class ArchiveEventsJob
{
    private readonly TenantDbContext _tenantDbContext;
    private readonly IArchiveRecordService _archiveRecordService;
    private readonly ITenantContext _tenantContext;

    public ArchiveEventsJob(
        TenantDbContext tenantDbContext,
        IArchiveRecordService archiveRecordService,
        ITenantContext tenantContext)
    {
        _tenantDbContext = tenantDbContext;
        _archiveRecordService = archiveRecordService;
        _tenantContext = tenantContext;
    }

    public async Task ArchiveFinishedEvents()
    {
        var now = DateTime.UtcNow;

        var finishedEvents = await _tenantDbContext.Events
            .AsNoTracking()
            .Where(x => x.EndUtc < now)
            .ToListAsync();

        var existingArchivedEvents =
            await _archiveRecordService.GetByEntityAsync("Event");

        var existingIds = existingArchivedEvents
            .Where(x => x.TenantId == _tenantContext.TenantId)
            .Select(x => x.EntityId)
            .ToHashSet();

        foreach (var ev in finishedEvents)
        {
            if (existingIds.Contains(ev.Id))
                continue;

            var dto = new CreateArchiveRecordDTO
            {
                TenantId = _tenantContext.TenantId,
                SchemaName = _tenantContext.SchemaName,
                EntityName = "Event",
                EntityId = ev.Id,
                ArchiveYear = ev.EndUtc.Year,
                DataJson = JsonSerializer.Serialize(ev)
            };

            await _archiveRecordService.CreateAsync(dto);
        }
    }
}