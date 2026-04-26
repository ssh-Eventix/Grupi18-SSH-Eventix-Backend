using Eventix.Application.DTOs.VenueSections;

namespace Eventix.Application.Interfaces.Services;

public interface IVenueSectionService
{
    Task<IEnumerable<VenueSectionResponseDTO>> GetAllAsync();

    Task<VenueSectionResponseDTO?> GetByIdAsync(Guid id);

    Task<IEnumerable<VenueSectionResponseDTO>> GetByVenueIdAsync(Guid venueId);

    Task<VenueSectionResponseDTO> CreateAsync(CreateVenueSectionDTO dto);

    Task<bool> UpdateAsync(Guid id, UpdateVenueSectionDTO dto);

    Task<bool> DeleteAsync(Guid id);
}