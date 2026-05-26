using Eventix.Application.DTOs.Events;

namespace Eventix.Application.Interfaces.Services;

public interface IPublicEventService
{
    Task<List<EventResponseDTO>> GetAllPublicAsync(
        string? search,
        CancellationToken cancellationToken = default);

    Task<EventResponseDTO?> GetPublicByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}