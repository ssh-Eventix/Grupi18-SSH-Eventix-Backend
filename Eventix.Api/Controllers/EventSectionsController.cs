using Eventix.Application.DTOs.EventSections;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Eventix.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventSectionsController : ControllerBase
{
    private readonly IEventSectionRepository _repository;

    public EventSectionsController(IEventSectionRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var eventSections = await _repository.GetAllAsync(cancellationToken);

        var response = eventSections.Select(MapToResponseDto);

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var eventSection = await _repository.GetByIdAsync(id, cancellationToken);

        if (eventSection is null)
            return NotFound();

        return Ok(MapToResponseDto(eventSection));
    }

    [HttpGet("event/{eventId:guid}")]
    public async Task<IActionResult> GetByEventId(Guid eventId, CancellationToken cancellationToken)
    {
        var eventSections = await _repository.GetByEventIdAsync(eventId, cancellationToken);

        var response = eventSections.Select(MapToResponseDto);

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEventSectionDTO dto, CancellationToken cancellationToken)
    {
        var exists = await _repository.ExistsByEventAndVenueSectionAsync(dto.EventId, dto.VenueSectionId, cancellationToken);

        if (exists)
        {
            return BadRequest(new
            {
                message = "An event section already exists for this event and venue section."
            });
        }

        var eventSection = new EventSection
        {
            EventId = dto.EventId,
            VenueSectionId = dto.VenueSectionId,
            Name = dto.Name,
            Code = dto.Code,
            Capacity = dto.Capacity,
            Price = dto.Price,
            Currency = dto.Currency,
            IsActive = dto.IsActive,
            IsHidden = dto.IsHidden,
            SalesEnabled = dto.SalesEnabled,
            SalesStartUtc = dto.SalesStartUtc,
            SalesEndUtc = dto.SalesEndUtc,
            MaxTicketsPerOrder = dto.MaxTicketsPerOrder,
            MinTicketsPerOrder = dto.MinTicketsPerOrder,
            Benefits = dto.Benefits,
            Notes = dto.Notes
        };

        await _repository.AddAsync(eventSection, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = eventSection.Id },
            MapToResponseDto(eventSection));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEventSectionDTO dto, CancellationToken cancellationToken)
    {
        var eventSection = await _repository.GetByIdAsync(id, cancellationToken);

        if (eventSection is null)
            return NotFound();

        eventSection.Name = dto.Name;
        eventSection.Code = dto.Code;
        eventSection.Capacity = dto.Capacity;
        eventSection.Price = dto.Price;
        eventSection.Currency = dto.Currency;
        eventSection.IsActive = dto.IsActive;
        eventSection.IsHidden = dto.IsHidden;
        eventSection.SalesEnabled = dto.SalesEnabled;
        eventSection.SalesStartUtc = dto.SalesStartUtc;
        eventSection.SalesEndUtc = dto.SalesEndUtc;
        eventSection.MaxTicketsPerOrder = dto.MaxTicketsPerOrder;
        eventSection.MinTicketsPerOrder = dto.MinTicketsPerOrder;
        eventSection.Benefits = dto.Benefits;
        eventSection.Notes = dto.Notes;

        await _repository.UpdateAsync(eventSection, cancellationToken);

        return Ok(MapToResponseDto(eventSection));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var eventSection = await _repository.GetByIdAsync(id, cancellationToken);

        if (eventSection is null)
            return NotFound();

        await _repository.DeleteAsync(eventSection, cancellationToken);

        return NoContent();
    }

    private static EventSectionResponseDTO MapToResponseDto(EventSection eventSection)
    {
        return new EventSectionResponseDTO
        {
            Id = eventSection.Id,
            TenantId = eventSection.TenantId,
            EventId = eventSection.EventId,
            VenueSectionId = eventSection.VenueSectionId,
            Name = eventSection.Name,
            Code = eventSection.Code,
            Capacity = eventSection.Capacity,
            ReservedSeats = eventSection.ReservedSeats,
            SoldSeats = eventSection.SoldSeats,
            Price = eventSection.Price,
            Currency = eventSection.Currency,
            IsActive = eventSection.IsActive,
            IsHidden = eventSection.IsHidden,
            SalesEnabled = eventSection.SalesEnabled,
            SalesStartUtc = eventSection.SalesStartUtc,
            SalesEndUtc = eventSection.SalesEndUtc,
            MaxTicketsPerOrder = eventSection.MaxTicketsPerOrder,
            MinTicketsPerOrder = eventSection.MinTicketsPerOrder,
            Benefits = eventSection.Benefits,
            Notes = eventSection.Notes,
            CreatedAtUtc = eventSection.CreatedAtUtc
        };
    }
}