using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventix.Application.DTOs.EventSections
{
    public class UpdateEventSectionDTO
    {
        public string Name { get; set; } = default!;
        public string Code { get; set; } = default!;

        public int Capacity { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; } = "EUR";

        public bool IsActive { get; set; }
        public bool IsHidden { get; set; }
        public bool SalesEnabled { get; set; }

        public DateTime? SalesStartUtc { get; set; }
        public DateTime? SalesEndUtc { get; set; }

        public int MaxTicketsPerOrder { get; set; }
        public int MinTicketsPerOrder { get; set; }

        public string? Benefits { get; set; }
        public string? Notes { get; set; }
    }
}
