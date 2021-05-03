using System.Threading.Tasks;
using FastEvents.dbo;

namespace FastEvents.DataAccess.Interfaces
{
    public interface IEventRepository : IRepository<DataAccess.EfModels.Event, Event>
    {
        Task<bool> DeleteAlongWithReferences(long eventId);
    }
    
}