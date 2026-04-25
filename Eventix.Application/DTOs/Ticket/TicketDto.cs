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
    }
}
