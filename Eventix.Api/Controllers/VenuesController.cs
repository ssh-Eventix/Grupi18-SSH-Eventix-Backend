using Eventix.Application.DTOs.Venues;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.MultiTenancy;
using Microsoft.AspNetCore.Mvc;

namespace Eventix.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VenuesController : ControllerBase
{
    private readonly IVenueRepository _repository;
    private readonly ITenantContext _tenantContext;

    public VenuesController(IVenueRepository repository, ITenantContext tenantContext)
    {
        _repository = repository;
        _tenantContext = tenantContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<VenueResponseDTO>>> GetAll(CancellationToken cancellationToken)
    {
        var venues = await _repository.GetAllAsync(cancellationToken);

        var response = venues
            .Where(x => x.TenantId == _tenantContext.TenantId && !x.IsDeleted)
            .Select(MapToResponseDto)
            .ToList();

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<VenueResponseDTO>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var venue = await _repository.GetByIdAsync(id, cancellationToken);

        if (venue is null || venue.TenantId != _tenantContext.TenantId || venue.IsDeleted)
            return NotFound();

        return Ok(MapToResponseDto(venue));
    }

    [HttpPost]
    public async Task<ActionResult<VenueResponseDTO>> Create(
        [FromBody] CreateVenueDTO dto,
        CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;

        var venue = new Venue
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,

            Name = dto.Name,
            Code = dto.Code,
            Description = dto.Description,

            AddressLine1 = dto.AddressLine1,
            AddressLine2 = dto.AddressLine2,
            City = dto.City,
            State = dto.State,
            PostalCode = dto.PostalCode,
            Country = dto.Country,

            Latitude = dto.Latitude,
            Longitude = dto.Longitude,

            TotalCapacity = dto.TotalCapacity,

            ContactEmail = dto.ContactEmail,
            ContactPhone = dto.ContactPhone,

            HasSeatingMap = dto.HasSeatingMap,
            SeatingMapImageUrl = dto.SeatingMapImageUrl,

            IsIndoor = dto.IsIndoor,
            IsAccessible = dto.IsAccessible,

            CreatedAtUtc = DateTime.UtcNow,
            IsDeleted = false
        };

        await _repository.AddAsync(venue, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = venue.Id }, MapToResponseDto(venue));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<VenueResponseDTO>> Update(
        Guid id,
        [FromBody] UpdateVenueDTO dto,
        CancellationToken cancellationToken)
    {
        var venue = await _repository.GetByIdAsync(id, cancellationToken);

        if (venue is null || venue.TenantId != _tenantContext.TenantId || venue.IsDeleted)
            return NotFound();

        venue.Name = dto.Name;
        venue.Code = dto.Code;
        venue.Description = dto.Description;

        venue.AddressLine1 = dto.AddressLine1;
        venue.AddressLine2 = dto.AddressLine2;
        venue.City = dto.City;
        venue.State = dto.State;
        venue.PostalCode = dto.PostalCode;
        venue.Country = dto.Country;

        venue.Latitude = dto.Latitude;
        venue.Longitude = dto.Longitude;

        venue.TotalCapacity = dto.TotalCapacity;

        venue.ContactEmail = dto.ContactEmail;
        venue.ContactPhone = dto.ContactPhone;

        venue.HasSeatingMap = dto.HasSeatingMap;
        venue.SeatingMapImageUrl = dto.SeatingMapImageUrl;

        venue.IsIndoor = dto.IsIndoor;
        venue.IsAccessible = dto.IsAccessible;
        venue.IsDeleted = dto.IsDeleted;

        venue.UpdatedAtUtc = DateTime.UtcNow;

        await _repository.UpdateAsync(venue);
        await _repository.SaveChangesAsync(cancellationToken);

        return Ok(MapToResponseDto(venue));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var venue = await _repository.GetByIdAsync(id, cancellationToken);

        if (venue is null || venue.TenantId != _tenantContext.TenantId || venue.IsDeleted)
            return NotFound();

        venue.IsDeleted = true;
        venue.UpdatedAtUtc = DateTime.UtcNow;

        await _repository.UpdateAsync(venue);
        await _repository.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    private static VenueResponseDTO MapToResponseDto(Venue venue)
    {
        return new VenueResponseDTO
        {
            Id = venue.Id,
            TenantId = venue.TenantId,

            Name = venue.Name,
            Code = venue.Code,
            Description = venue.Description,

            AddressLine1 = venue.AddressLine1,
            AddressLine2 = venue.AddressLine2,
            City = venue.City,
            State = venue.State,
            PostalCode = venue.PostalCode,
            Country = venue.Country,

            Latitude = venue.Latitude,
            Longitude = venue.Longitude,

            TotalCapacity = venue.TotalCapacity,
            HasSeatingMap = venue.HasSeatingMap,
            SeatingMapImageUrl = venue.SeatingMapImageUrl,

            IsIndoor = venue.IsIndoor,
            IsAccessible = venue.IsAccessible,

            CreatedAtUtc = venue.CreatedAtUtc
        };
    }
}