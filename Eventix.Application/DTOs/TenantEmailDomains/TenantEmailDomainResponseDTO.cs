using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventix.Application.DTOs.TenantEmailDomains
{
    public class TenantEmailDomainResponseDTO
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string Domain { get; set; } = string.Empty;
        public string DefaultRoleName { get; set; } = string.Empty;
        public bool AutoApprove { get; set; }
    }
}
