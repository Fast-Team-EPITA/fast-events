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
            throw new NotImplementedException();
        }

        public List<dbo.Event> GetByOwnerId(string ownerId)
        {
            throw new NotImplementedException();
        }
    }
}
