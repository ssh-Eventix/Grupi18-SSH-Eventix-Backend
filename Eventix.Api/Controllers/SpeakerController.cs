using Eventix.Application.DTOs.Speakers;
using Eventix.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Eventix.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpeakersController : ControllerBase
{
    private readonly ISpeakerService _speakerService;

    public SpeakersController(ISpeakerService speakerService)
    {
        _speakerService = speakerService;
    }

    [HttpGet]
    public async Task<ActionResult<List<SpeakerDto>>> GetAll(CancellationToken cancellationToken)
    {
        var speakers = await _speakerService.GetAllAsync(cancellationToken);
        return Ok(speakers);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SpeakerDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var speaker = await _speakerService.GetByIdAsync(id, cancellationToken);

        if (speaker is null)
            return NotFound();

        return Ok(speaker);
    }

    [HttpPost]
    public async Task<ActionResult<SpeakerDto>> Create(
        [FromBody] CreateSpeakerDto dto,
        CancellationToken cancellationToken)
    {
        var createdSpeaker = await _speakerService.CreateAsync(dto, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = createdSpeaker.Id }, createdSpeaker);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateSpeakerDto dto,
        CancellationToken cancellationToken)
    {
        var updated = await _speakerService.UpdateAsync(id, dto, cancellationToken);

        if (!updated)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _speakerService.DeleteAsync(id, cancellationToken);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}

