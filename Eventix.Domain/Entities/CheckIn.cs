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
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid TicketId { get; set; }

        [ForeignKey(nameof(TicketId))]
        public Ticket Ticket { get; set; }

        [Required]
        public Guid CheckedInByUserId { get; set; }

        [ForeignKey(nameof(CheckedInByUserId))]
        public User CheckedInByUser { get; set; }

        [Required]
        public DateTime CheckInTime { get; set; }

        public string Notes { get; set; }
    }
}
