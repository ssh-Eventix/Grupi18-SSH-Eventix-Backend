using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventix.Application.DTOs.EventSections
{
    public class CreateEventSectionDTO
    {
        public Guid EventId { get; set; }
        public Guid VenueSectionId { get; set; }

        public string Name { get; set; } = default!;
        public string Code { get; set; } = default!;

        public int Capacity { get; set; }
        public decimal Price { get; set; }

        public DateTime? SalesStartUtc { get; set; }
        public DateTime? SalesEndUtc { get; set; }
    }
}
