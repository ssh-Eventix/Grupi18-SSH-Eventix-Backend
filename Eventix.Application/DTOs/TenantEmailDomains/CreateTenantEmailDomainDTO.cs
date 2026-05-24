using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventix.Application.DTOs.TenantEmailDomains
{
    public class CreateTenantEmailDomainDTO
    {
        public Guid TenantId { get; set; }
        public string Domain { get; set; } = string.Empty;
        public string DefaultRoleName { get; set; } = "Buyer";
        public bool AutoApprove { get; set; } = true;
    }
}
