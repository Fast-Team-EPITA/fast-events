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
    public class TicketRepository : Repository<EfModels.Ticket, dbo.Ticket>, Interfaces.ITicketsRepository
    {
        public TicketRepository(FastEventContext context, ILogger<EventRepository> logger, IMapper mapper) : base(context, logger, mapper)
        {
        }

        public int GetNbBookedByEventId(long eventId)
        {
            var result = _context.Tickets.Where(x => x.EventId == eventId);
            return result.Count();
        }

        List<dbo.Ticket> ITicketsRepository.GetByOwnerId(string ownerId)
        {
            var result = _context.Tickets.Where(x => x.OwnerUuid == ownerId);
            return _mapper.Map<List<dbo.Ticket>>(result);
        }
    }
}
