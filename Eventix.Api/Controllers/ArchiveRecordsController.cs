using Eventix.Application.DTOs.Archive;
using Eventix.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eventix.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ArchiveRecordsController : ControllerBase
{
    private readonly IArchiveRecordService _service;

    public ArchiveRecordsController(IArchiveRecordService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Policy = "Permission:ViewArchiveRecords")]
    public async Task<IActionResult> GetAll()
    {
        var records = await _service.GetAllAsync();
        return Ok(records);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Permission:ViewArchiveRecords")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var record = await _service.GetByIdAsync(id);

        if (record == null)
            return NotFound();

        return Ok(record);
    }

    [HttpGet("entity/{entityName}")]
    [Authorize(Policy = "Permission:ViewArchiveRecords")]
    public async Task<IActionResult> GetByEntity(string entityName)
    {
        var records = await _service.GetByEntityAsync(entityName);
        return Ok(records);
    }

    [HttpGet("year/{year:int}")]
    [Authorize(Policy = "Permission:ViewArchiveRecords")]
    public async Task<IActionResult> GetByYear(int year)
    {
        var records = await _service.GetByYearAsync(year);
        return Ok(records);
    }

    [HttpGet("stats")]
    [Authorize(Policy = "Permission:ViewArchiveRecords")]
    public async Task<IActionResult> GetStats()
    {
        var records = await _service.GetAllAsync();

        var stats = new
        {
            totalArchived = records.Count,
            archivedEvents = records.Count(x => x.EntityName == "Event"),
            archivedThisYear = records.Count(x => x.ArchiveYear == DateTime.UtcNow.Year),
            byEntity = records
                .GroupBy(x => x.EntityName)
                .Select(g => new { entityName = g.Key, count = g.Count() })
                .OrderByDescending(x => x.count),
            eventsByYear = records
                .Where(x => x.EntityName == "Event")
                .GroupBy(x => x.ArchiveYear)
                .Select(g => new { year = g.Key, count = g.Count() })
                .OrderBy(x => x.year)
        };

        return Ok(stats);
    }
}