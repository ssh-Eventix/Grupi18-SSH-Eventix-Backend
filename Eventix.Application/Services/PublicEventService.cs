using Eventix.Application.DTOs.Events;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;

namespace Eventix.Application.Services;

public class PublicEventService : IPublicEventService
{
    private readonly IPublicEventRepository _repository;

    public PublicEventService(IPublicEventRepository repository)
    {
        _repository = repository;
    }

    public Task<List<EventResponseDTO>> GetAllPublicAsync(
        string? search,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetAllPublicAsync(search, cancellationToken);
    }

    public Task<EventResponseDTO?> GetPublicByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetPublicByIdAsync(id, cancellationToken);
    }
}