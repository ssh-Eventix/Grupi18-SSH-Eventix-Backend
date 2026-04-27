using Eventix.Application.DTOs.EventSessions;
using Eventix.Application.Interfaces.Common;
using Eventix.Domain.Entities;

public class EventSessionService : IEventSessionService
{
    private readonly IEventSessionRepository _repository;
    private readonly ITenantContext _tenantContext;

    public EventSessionService(IEventSessionRepository repository, ITenantContext tenantContext)
    {
        _repository = repository;
        _tenantContext = tenantContext;
    }

    public async Task<List<EventSessionResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return items.Select(Map).ToList();
    }

    public async Task<EventSessionResponseDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : Map(entity);
    }

    public async Task<List<EventSessionResponseDTO>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetByEventIdAsync(eventId, cancellationToken);
        return items.Select(Map).ToList();
    }

    public async Task<EventSessionResponseDTO> CreateAsync(CreateEventSessionDTO dto, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var entity = new EventSession
        {
            Id = Guid.NewGuid(),
            TenantId = _tenantContext.TenantId,
            EventId = dto.EventId,
            SpeakerId = dto.SpeakerId,
            Title = dto.Title,
            Description = dto.Description,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _repository.AddAsync(entity, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Map(entity);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateEventSessionDTO dto, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return false;

        entity.EventId = dto.EventId;
        entity.SpeakerId = dto.SpeakerId;
        entity.Title = dto.Title;
        entity.Description = dto.Description;
        entity.StartTime = dto.StartTime;
        entity.EndTime = dto.EndTime;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _repository.UpdateAsync(entity);
        await _repository.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return false;

        entity.IsDeleted = true;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _repository.UpdateAsync(entity);
        await _repository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static EventSessionResponseDTO Map(EventSession x) => new()
    {
        Id = x.Id,
        EventId = x.EventId,
        Title = x.Title,
        Description = x.Description,
        StartTime = x.StartTime,
        EndTime = x.EndTime,
        SpeakerId = x.SpeakerId,
        CreatedAtUtc = x.CreatedAtUtc,
        UpdatedAtUtc = x.UpdatedAtUtc
    };
}