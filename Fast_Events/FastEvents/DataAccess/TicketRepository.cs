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
    public class TicketRepository : Repository<EfModels.Ticket, dbo.Ticket>, Interfaces.IEventRepository
    {
        public TicketRepository(FastEventContext context, ILogger<EventRepository> logger, IMapper mapper) : base(context, logger, mapper)
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

        public Task<dbo.Event> Insert(dbo.Event entity)
        {
            throw new NotImplementedException();
        }

        public Task<dbo.Event> Update(dbo.Event entity)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<dbo.Event>> IRepository<EfModels.Event, dbo.Event>.Get(string includeTables)
        {
            throw new NotImplementedException();
        }
    }
}
