using Eventix.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventix.Domain.Entities
{
    public class TicketType : BaseEntity
    {
        public Guid EventId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int QuantityAvailable { get; set; }
        public int SoldQuantity { get; set; }
        public DateTime SaleStartDate { get; set; }
        public DateTime SaleEndDate { get; set; }


        // Navigation property
        public Event Event { get; set; }
        public ICollection<BookingItem> BookingItems { get; set; } = new List<BookingItem>();
    }
}
