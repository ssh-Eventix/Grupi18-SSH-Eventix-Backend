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

    [HttpPost]
    [Authorize(Policy = "Permission:ManageArchiveRecords")]
    public async Task<IActionResult> Create(CreateArchiveRecordDTO dto)
    {
        var record = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = record.Id }, record);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Permission:ManageArchiveRecords")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}