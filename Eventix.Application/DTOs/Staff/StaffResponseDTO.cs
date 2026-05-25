using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventix.Application.DTOs.Staff
{
    public class StaffResponseDTO
    {
        public Guid Id { get; set; }
        public Guid PublicUserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = "Staff";
        public bool IsActive { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
