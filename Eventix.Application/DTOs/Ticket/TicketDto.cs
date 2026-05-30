using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventix.Application.DTOs.Ticket
{
    public class TicketDto
    {
        public Guid Id { get; set; }
        public string TicketCode { get; set; } = string.Empty;
        public string QRCode { get; set; } = string.Empty;
        public int Status { get; set; }
        public DateTime IssuedAt { get; set; }
        public DateTime? UsedAt { get; set; }
        public Guid? BookingId { get; set; }
        public Guid? EventId { get; set; }
        public string? EventTitle { get; set; }
        public string? BuyerEmail { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? BookingStatus { get; set; }
    }
}
