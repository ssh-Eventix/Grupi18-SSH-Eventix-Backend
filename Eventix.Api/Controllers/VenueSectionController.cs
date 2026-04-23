using Eventix.Application.DTOs.VenueSections;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Domain.Enums;
using Eventix.Infrastructure.MultiTenancy;
using Microsoft.AspNetCore.Mvc;

namespace Eventix.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VenueSectionsController : ControllerBase
{
    private readonly IVenueSectionRepository _repository;
    private readonly IVenueRepository _venueRepository;
    private readonly ITenantContext _tenantContext;

    public VenueSectionsController(
        IVenueSectionRepository repository,
        IVenueRepository venueRepository,
        ITenantContext tenantContext)
    {
        _repository = repository;
        _venueRepository = venueRepository;
        _tenantContext = tenantContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<VenueSectionResponseDTO>>> GetAll(CancellationToken cancellationToken)
    {
        var sections = await _repository.GetAllAsync(cancellationToken);

        var response = sections
            .Where(x => x.TenantId == _tenantContext.TenantId && !x.IsDeleted)
            .OrderBy(x => x.DisplayOrder)
            .Select(MapToResponseDto)
            .ToList();

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<VenueSectionResponseDTO>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var section = await _repository.GetByIdAsync(id, cancellationToken);

        if (section is null || section.TenantId != _tenantContext.TenantId || section.IsDeleted)
            return NotFound();

        return Ok(MapToResponseDto(section));
    }

    [HttpGet("venue/{venueId:guid}")]
    public async Task<ActionResult<IEnumerable<VenueSectionResponseDTO>>> GetByVenueId(Guid venueId, CancellationToken cancellationToken)
    {
        var venue = await _venueRepository.GetByIdAsync(venueId, cancellationToken);

        if (venue is null || venue.TenantId != _tenantContext.TenantId || venue.IsDeleted)
            return NotFound("Venue not found.");

        var sections = await _repository.GetByVenueIdAsync(venueId, cancellationToken);

        var response = sections
            .Where(x => x.TenantId == _tenantContext.TenantId && !x.IsDeleted)
            .OrderBy(x => x.DisplayOrder)
            .Select(MapToResponseDto)
            .ToList();

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<VenueSectionResponseDTO>> Create(
        [FromBody] CreateVenueSectionDTO dto,
        CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;

        var section = new VenueSection
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            VenueId = dto.VenueId,

            Name = dto.Name,
            Code = dto.Code,
            Description = dto.Description,

            Capacity = dto.Capacity,
            SeatType = (SeatType)dto.SeatType,

            RowCount = dto.RowCount,
            SeatsPerRow = dto.SeatsPerRow,

            DisplayOrder = dto.DisplayOrder,
            IsAccessibleSection = dto.IsAccessibleSection,
            IsActive = dto.IsActive,

            DefaultBasePrice = dto.DefaultBasePrice,

            CreatedAtUtc = DateTime.UtcNow,
            IsDeleted = false
        };

        await _repository.AddAsync(section, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = section.Id }, MapToResponseDto(section));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<VenueSectionResponseDTO>> Update(
        Guid id,
        [FromBody] UpdateVenueSectionDTO dto,
        CancellationToken cancellationToken)
    {
        var section = await _repository.GetByIdAsync(id, cancellationToken);

        if (section is null || section.TenantId != _tenantContext.TenantId || section.IsDeleted)
            return NotFound();

        section.Name = dto.Name;
        section.Code = dto.Code;
        section.Description = dto.Description;

        section.Capacity = dto.Capacity;
        section.SeatType = (SeatType)dto.SeatType;

        section.RowCount = dto.RowCount;
        section.SeatsPerRow = dto.SeatsPerRow;

        section.DisplayOrder = dto.DisplayOrder;
        section.IsAccessibleSection = dto.IsAccessibleSection;
        section.IsActive = dto.IsActive;

        section.DefaultBasePrice = dto.DefaultBasePrice;

        section.UpdatedAtUtc = DateTime.UtcNow;

        await _repository.UpdateAsync(section);
        await _repository.SaveChangesAsync(cancellationToken);

        return Ok(MapToResponseDto(section));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var section = await _repository.GetByIdAsync(id, cancellationToken);

        if (section is null || section.TenantId != _tenantContext.TenantId || section.IsDeleted)
            return NotFound();

        section.IsDeleted = true;
        section.UpdatedAtUtc = DateTime.UtcNow;

        await _repository.UpdateAsync(section);
        await _repository.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    private static VenueSectionResponseDTO MapToResponseDto(VenueSection section)
    {
        return new VenueSectionResponseDTO
        {
            Id = section.Id,
            TenantId = section.TenantId,
            VenueId = section.VenueId,

            Name = section.Name,
            Code = section.Code,
            Description = section.Description,

            Capacity = section.Capacity,
            SeatType = (int)section.SeatType,

            RowCount = section.RowCount,
            SeatsPerRow = section.SeatsPerRow,

            DisplayOrder = section.DisplayOrder,
            IsAccessibleSection = section.IsAccessibleSection,
            IsActive = section.IsActive,

            DefaultBasePrice = section.DefaultBasePrice,

            CreatedAtUtc = section.CreatedAtUtc
        };
    }
}