using Eventix.Application.DTOs.TenantAdmins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventix.Application.Interfaces.Services
{
    public interface ITenantAdminService
    {
        Task<TenantAdminResponseDTO> CreateAsync(CreateTenantAdminDTO dto, CancellationToken ct);
    }
}
