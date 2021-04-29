using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastEvents.DataAccess.Interfaces
{
    public interface IStatRepository : DataAccess.IRepository<EfModels.Stat, dbo.Stat>
    {
        dbo.StatByEvent GetByEvent(long eventId); // TODO changer le nom
    }
}
