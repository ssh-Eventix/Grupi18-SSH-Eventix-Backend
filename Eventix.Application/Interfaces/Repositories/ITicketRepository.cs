using Eventix.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventix.Application.Interfaces.Repositories
{
    public interface ITicketRepository
    {
        Task AddRangeAsync(List<Ticket> tickets);
    }
}
