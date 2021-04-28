using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastEvents.DataAccess.EfModels;
using Microsoft.Extensions.Logging;
using AutoMapper;
using FastEvents.dbo;

namespace FastEvents.DataAccess
{
    public class EventRepository : Repository<EfModels.Event, dbo.Event>, Interfaces.IEventRepository
    {
        public EventRepository(FastEventContext context, ILogger<EventRepository> logger, IMapper mapper) : base(context, logger, mapper)
        {
        }

        public List<dbo.Event> GetByCategory(Category category)
        {
            var result = _context.Events.Where(x => x.Category.Equals(category));
            return _mapper.Map < List<dbo.Event>>(result);

        }

        public List<dbo.Event> GetByOwnerId(string ownerId)
        {
            var result = _context.Events.Where(x => x.OwnerUuid == ownerId);
            return _mapper.Map<List<dbo.Event>>(result);
        }
    }
}
