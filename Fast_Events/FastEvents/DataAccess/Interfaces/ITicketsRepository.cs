using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastEvents.DataAccess.Interfaces
{
    public interface ITicketsRepository : DataAccess.IRepository<EfModels.Ticket, dbo.Ticket>
    {
        List<dbo.Ticket> GetByOwnerId(string ownerId);
        int GetNbBookedByEventId(long eventId);
    }
}
