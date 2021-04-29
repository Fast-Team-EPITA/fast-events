using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastEvents.DataAccess.Interfaces
{
    public interface IEventRepository : DataAccess.IRepository<EfModels.EventView, dbo.EventUi>
    {
        public dbo.EventUi GetById(long id);
        List<dbo.EventUi> GetByOwnerId(string ownerId);
        List<dbo.EventUi> GetByCategory(dbo.Category category);
    }
}
