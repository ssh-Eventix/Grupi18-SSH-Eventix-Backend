using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventix.Application.DTOs.TicketType
{
    public class TicketTypeDto
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public Guid EventSectionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int QuantityAvailable { get; set; }
        public int SoldQuantity { get; set; }
        public DateTime SaleStartDate { get; set; }
        public DateTime SaleEndDate { get; set; }
    }
}
