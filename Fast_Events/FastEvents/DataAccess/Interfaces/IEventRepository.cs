using FastEvents.dbo;

namespace FastEvents.DataAccess.Interfaces
{
    public interface IEventRepository : IRepository<DataAccess.EfModels.Event, Event>
    {
    }
}