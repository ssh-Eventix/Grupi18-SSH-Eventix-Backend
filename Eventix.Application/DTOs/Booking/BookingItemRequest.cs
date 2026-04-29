using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventix.Application.DTOs.Booking
{
    public class BookingItemRequest
    {
        public Guid TicketId { get; set; }
        public int Quantity { get; set; }
    }
}
