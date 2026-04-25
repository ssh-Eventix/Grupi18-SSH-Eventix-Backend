using Eventix.Application.DTOs.Ticket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventix.Application.DTOs.Booking
{
    public class BookingDto
    {
        public Guid Id { get; set; }
        public string ReferenceNumber { get; set; } = string.Empty; 
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;

        public List<TicketDto> Tickets { get; set; } = new();
    }
}
