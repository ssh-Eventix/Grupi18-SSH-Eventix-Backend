using Eventix.Application.DTOs.Venues;

namespace Eventix.Application.Interfaces.Services;

public interface IVenueService
{
    Task<IEnumerable<VenueResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<VenueResponseDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<VenueResponseDTO> CreateAsync(CreateVenueDTO dto, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(Guid id, UpdateVenueDTO dto, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}