using Eventix.Application.DTOs.Events;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Domain.Enums;
using Eventix.Infrastructure.MultiTenancy;
using Microsoft.AspNetCore.Mvc;

namespace Eventix.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IEventRepository _repository;
    private readonly ITenantContext _tenantContext;

    public EventsController(
        IEventRepository repository,
        ITenantContext tenantContext)
    {
        _repository = repository;
        _tenantContext = tenantContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EventResponseDTO>>> GetAll(
        [FromQuery] string? search,
        CancellationToken cancellationToken)
    {
        var entities = await _repository.GetAllAsync(search, cancellationToken);

        var response = entities
            .Where(x => x.TenantId == _tenantContext.TenantId && !x.IsDeleted)
            .OrderByDescending(x => x.StartUtc)
            .Select(MapToResponseDto)
            .ToList();

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EventResponseDTO>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);

        if (entity is null || entity.TenantId != _tenantContext.TenantId || entity.IsDeleted)
            return NotFound();

        return Ok(MapToResponseDto(entity));
    }

    [HttpPost]
    public async Task<ActionResult<EventResponseDTO>> Create(
        [FromBody] CreateEventDTO dto,
        CancellationToken cancellationToken)
    {
        var entity = new Event
        {
            Id = Guid.NewGuid(),
            TenantId = _tenantContext.TenantId,
            VenueId = dto.VenueId,
            EventCategoryId = dto.EventCategoryId,

            Title = dto.Title,
            Slug = dto.Slug,
            Subtitle = dto.Subtitle,
            Description = dto.Description,
            ShortDescription = dto.ShortDescription,

            OrganizerName = dto.OrganizerName,
            OrganizerEmail = dto.OrganizerEmail,
            OrganizerPhone = dto.OrganizerPhone,

            StartUtc = dto.StartUtc,
            EndUtc = dto.EndUtc,
            DoorsOpenUtc = dto.DoorsOpenUtc,
            SalesStartUtc = dto.SalesStartUtc,
            SalesEndUtc = dto.SalesEndUtc,

            Status = EventStatus.Draft,
            Visibility = (EventVisibility)dto.Visibility,

            BannerImageUrl = dto.BannerImageUrl,
            ThumbnailImageUrl = dto.ThumbnailImageUrl,

            Tags = dto.Tags,

            MaxTicketsPerOrder = dto.MaxTicketsPerOrder,
            MinTicketsPerOrder = dto.MinTicketsPerOrder,

            IsFree = dto.IsFree,
            IsPublished = false,
            AllowWaitlist = dto.AllowWaitlist,
            RequiresApproval = dto.RequiresApproval,

            TermsAndConditions = dto.TermsAndConditions,
            RefundPolicy = dto.RefundPolicy,

            Currency = dto.Currency,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _repository.AddAsync(entity, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, MapToResponseDto(entity));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateEventDTO dto,
        CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);

        if (entity is null || entity.TenantId != _tenantContext.TenantId || entity.IsDeleted)
            return NotFound();

        entity.VenueId = dto.VenueId;
        entity.EventCategoryId = dto.EventCategoryId;

        entity.Title = dto.Title;
        entity.Slug = dto.Slug;
        entity.Subtitle = dto.Subtitle;
        entity.Description = dto.Description;
        entity.ShortDescription = dto.ShortDescription;

        entity.OrganizerName = dto.OrganizerName;
        entity.OrganizerEmail = dto.OrganizerEmail;
        entity.OrganizerPhone = dto.OrganizerPhone;

        entity.StartUtc = dto.StartUtc;
        entity.EndUtc = dto.EndUtc;
        entity.DoorsOpenUtc = dto.DoorsOpenUtc;
        entity.SalesStartUtc = dto.SalesStartUtc;
        entity.SalesEndUtc = dto.SalesEndUtc;

        entity.Status = (EventStatus)dto.Status;
        entity.Visibility = (EventVisibility)dto.Visibility;

        entity.BannerImageUrl = dto.BannerImageUrl;
        entity.ThumbnailImageUrl = dto.ThumbnailImageUrl;

        entity.Tags = dto.Tags;

        entity.MaxTicketsPerOrder = dto.MaxTicketsPerOrder;
        entity.MinTicketsPerOrder = dto.MinTicketsPerOrder;

        entity.IsFree = dto.IsFree;
        entity.IsPublished = dto.IsPublished;
        entity.AllowWaitlist = dto.AllowWaitlist;
        entity.RequiresApproval = dto.RequiresApproval;

        entity.TermsAndConditions = dto.TermsAndConditions;
        entity.RefundPolicy = dto.RefundPolicy;

        entity.Currency = dto.Currency;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _repository.UpdateAsync(entity);
        await _repository.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);

        if (entity is null || entity.TenantId != _tenantContext.TenantId || entity.IsDeleted)
            return NotFound();

        entity.IsDeleted = true;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _repository.UpdateAsync(entity);
        await _repository.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    private static EventResponseDTO MapToResponseDto(Event entity)
    {
        return new EventResponseDTO
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            VenueId = entity.VenueId,
            EventCategoryId = entity.EventCategoryId,

            Title = entity.Title,
            Slug = entity.Slug,
            Subtitle = entity.Subtitle,
            Description = entity.Description,
            ShortDescription = entity.ShortDescription,

            OrganizerName = entity.OrganizerName,

            StartUtc = entity.StartUtc,
            EndUtc = entity.EndUtc,
            DoorsOpenUtc = entity.DoorsOpenUtc,
            SalesStartUtc = entity.SalesStartUtc,
            SalesEndUtc = entity.SalesEndUtc,

            Status = entity.Status.ToString(),
            Visibility = entity.Visibility.ToString(),

            BannerImageUrl = entity.BannerImageUrl,
            ThumbnailImageUrl = entity.ThumbnailImageUrl,

            EventCategoryName = entity.EventCategory?.Name,
            Tags = entity.Tags,

            IsFree = entity.IsFree,
            IsPublished = entity.IsPublished,
            AllowWaitlist = entity.AllowWaitlist,
            RequiresApproval = entity.RequiresApproval,

            MinPrice = entity.MinPrice,
            MaxPrice = entity.MaxPrice,
            Currency = entity.Currency,

            CreatedAtUtc = entity.CreatedAtUtc
        };
    }
}