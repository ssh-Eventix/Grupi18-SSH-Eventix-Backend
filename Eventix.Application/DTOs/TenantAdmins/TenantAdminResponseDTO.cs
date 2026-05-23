using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventix.Application.DTOs.TenantAdmins
{
    public class TenantAdminResponseDTO
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = "Admin";
    }
}
