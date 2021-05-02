using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastEvents.DataAccess.EfModels;
using Microsoft.Extensions.Logging;
using AutoMapper;
using FastEvents.dbo;
using FastEvents.DataAccess.Interfaces;

namespace FastEvents.DataAccess
{
    public class TicketRepository : Repository<EfModels.Ticket, dbo.Ticket>, Interfaces.ITicketRepository
    {
        public TicketRepository(FastEventContext context, ILogger<TicketRepository> logger, IMapper mapper) : base(context, logger, mapper)
        {
        }

        public int GetNbBookedByEventId(long eventId)
        {
            return _context.Tickets.Count(x => x.EventId == eventId);
        }

        public List<dbo.Ticket> GetByOwnerId(string ownerId)
        {
            var result = _context.Tickets.Where(x => x.OwnerUuid == ownerId).ToList();
            foreach (var ticket in result)
                _context.Entry(ticket).Reference(r => r.Event).Load();
            return _mapper.Map<List<dbo.Ticket>>(result);
        }
    }
}
