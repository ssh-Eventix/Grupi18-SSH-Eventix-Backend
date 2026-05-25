using Eventix.Application.DTOs.Staff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventix.Application.Interfaces.Services
{
    public interface IStaffService
    {
        Task<List<StaffResponseDTO>> GetAllAsync(CancellationToken ct = default);
        Task<StaffResponseDTO> CreateAsync(CreateStaffDTO dto, CancellationToken ct = default);
        Task<bool> DeactivateAsync(Guid id, CancellationToken ct = default);
    }
}
