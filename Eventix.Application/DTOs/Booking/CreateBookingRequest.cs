using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventix.Application.DTOs.Booking
{
    public class CreateBookingRequest
    {
        public Guid UserId { get; set; }
        public Guid EventId { get; set; }

        public List<BookingItemRequest> BookingItems { get; set; } = new();
    }
}
