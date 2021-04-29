using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastEvents.DataAccess.Interfaces
{
    public interface IEventRepository : DataAccess.IRepository<EfModels.Event, dbo.Event>
    {
        public dbo.Event GetById(long id);
        List<dbo.Event> GetByOwnerId(string ownerId);
        List<dbo.Event> GetByCategory(dbo.Category category);
    }
}
