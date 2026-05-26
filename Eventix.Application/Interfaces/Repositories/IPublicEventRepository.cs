using Eventix.Application.DTOs.Events;

namespace Eventix.Application.Interfaces.Repositories;

public interface IPublicEventRepository
{
    Task<List<EventResponseDTO>> GetAllPublicAsync(
        string? search,
        CancellationToken cancellationToken = default);

    Task<EventResponseDTO?> GetPublicByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}