using Eventix.Application.DTOs.Events;

namespace Eventix.Application.Interfaces.Services;

public interface IEventService
{
    Task<IEnumerable<EventResponseDTO>> GetAllAsync(string? search = null, CancellationToken cancellationToken = default);
    Task<EventResponseDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<EventResponseDTO> CreateAsync(CreateEventDTO dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Guid id, UpdateEventDTO dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}