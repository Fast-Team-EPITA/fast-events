using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FastEvents.DataAccess.EfModels;
using Microsoft.Extensions.Logging;
using Event = FastEvents.dbo.Event;

namespace FastEvents.DataAccess
{
    public class EventRepository: Repository<DataAccess.EfModels.Event, Event>, Interfaces.IEventRepository
    {
        public EventRepository(FastEventContext context, ILogger<EventUiRepository> logger, IMapper mapper) : base(context, logger, mapper)
        {
        }

        public async Task<bool> DeleteAlongWithReferences(long eventId)
        {
            var associatedStats = _context.Stats.Where(stat => stat.EventId == eventId);
            _context.Stats.RemoveRange(associatedStats);
            var associatedTickets = _context.Tickets.Where(stat => stat.EventId == eventId);
            _context.Tickets.RemoveRange(associatedTickets);
            return await Delete(eventId);
        }
    }
}