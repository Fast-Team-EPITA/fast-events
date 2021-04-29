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
    }
}