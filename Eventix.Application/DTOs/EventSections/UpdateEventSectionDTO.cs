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

        public bool IsActive { get; set; }
    }
}
