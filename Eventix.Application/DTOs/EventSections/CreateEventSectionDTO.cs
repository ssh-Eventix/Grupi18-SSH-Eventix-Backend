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
        public string Currency { get; set; } = "EUR";

        public bool IsActive { get; set; } = true;
        public bool IsHidden { get; set; } = false;
        public bool SalesEnabled { get; set; } = true;

        public DateTime? SalesStartUtc { get; set; }
        public DateTime? SalesEndUtc { get; set; }

        public int MaxTicketsPerOrder { get; set; } = 10;
        public int MinTicketsPerOrder { get; set; } = 1;

        public string? Benefits { get; set; }
        public string? Notes { get; set; }
    }
}
