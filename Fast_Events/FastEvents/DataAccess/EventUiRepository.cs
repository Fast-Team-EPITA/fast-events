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
    public class EventUiRepository : Repository<EfModels.EventView, dbo.EventUi>, Interfaces.IEventUiRepository
    {
        public EventUiRepository(FastEventContext context, ILogger<EventUiRepository> logger, IMapper mapper) : base(context, logger, mapper)
        {
        }

        public dbo.EventUi GetById(long id)
        {
            var result = _context.EventViews.FirstOrDefault(x => x.Id == id) ?? new EfModels.EventView();
            return _mapper.Map<dbo.EventUi>(result);
        }

        public List<dbo.EventUi> GetByCategory(Category category)
        {
            var result = _context.Events.Where(x => x.Category == category.ToString());
            return _mapper.Map<List<dbo.EventUi>>(result);

        }

        public List<dbo.EventUi> GetByOwnerId(string ownerId)
        {
            var result = _context.Events.Where(x => x.OwnerUuid == ownerId);
            return _mapper.Map<List<dbo.EventUi>>(result);
        }
    }
}
