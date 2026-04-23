using Eventix.Application.DTOs.EventCategories;

namespace Eventix.Application.Interfaces.Services;

public interface IEventCategoryService
{
    Task<IEnumerable<EventCategoryResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<EventCategoryResponseDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<EventCategoryResponseDTO> CreateAsync(CreateEventCategoryDTO dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Guid id, UpdateEventCategoryDTO dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}