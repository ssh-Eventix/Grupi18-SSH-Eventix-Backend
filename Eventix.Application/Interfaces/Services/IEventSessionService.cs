using Eventix.Application.DTOs.EventSessions;

public interface IEventSessionService
{
    Task<List<EventSessionResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<EventSessionResponseDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<EventSessionResponseDTO>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task<EventSessionResponseDTO> CreateAsync(CreateEventSessionDTO dto, Guid tenantId, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Guid id, UpdateEventSessionDTO dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}