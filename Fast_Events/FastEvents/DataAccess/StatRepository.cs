using FastEvents.dbo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastEvents.DataAccess.EfModels;
using Microsoft.Extensions.Logging;
using AutoMapper;

namespace FastEvents.DataAccess
{
    public class StatRepository : Repository<EfModels.Stat, dbo.Stat>, Interfaces.IStatRepository
    {
        public StatRepository(FastEventContext context, ILogger<StatRepository> logger, IMapper mapper) : base(context, logger, mapper)
        {
        }

        public StatByEventModel GetByEvent(long eventId)
        {
            var result = _context.Stats.FirstOrDefault(x => x.EventId == eventId);
            return _mapper.Map<dbo.StatByEventModel>(result);
        }
    }
}
