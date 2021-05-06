using FastEvents.dbo.Interfaces;

#nullable disable

namespace FastEvents.DataAccess.EfModels
{
    public partial class StatByEvent: IObjectWithId
    {
        public int NumberView { get; set; }
        public string Name { get; set; }
        public long Id { get; set; }
    }
}
