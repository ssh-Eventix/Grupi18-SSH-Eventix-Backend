using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eventix.Application.DTOs.EventSections;

namespace Eventix.Application.Interfaces.Services
{
    public interface IEventSectionService
    {
        Task<IEnumerable<EventSectionResponseDTO>> GetAllAsync();
        Task<EventSectionResponseDTO?> GetByIdAsync(Guid id);
        Task<EventSectionResponseDTO> CreateAsync(CreateEventSectionDTO dto);
        Task<EventSectionResponseDTO?> UpdateAsync(Guid id, UpdateEventSectionDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
