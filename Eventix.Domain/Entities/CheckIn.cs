using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Eventix.Domain.Common;

namespace Eventix.Domain.Entities
{
    public class CheckIn : TenantBaseEntity
    {
        public Guid TicketId { get; set; }

        public Ticket Ticket { get; set; } = default!;

        public Guid CheckedInByUserId { get; set; }

        public User CheckedInByUser { get; set; } = default!;

        public DateTime CheckInTime { get; set; } = DateTime.UtcNow;

        public string? Notes { get; set; }
    }
}
