#nullable disable

namespace FastEvents.DataAccess.EfModels
{
    public partial class Ticket
    {
        public long Id { get; set; }
        public long EventId { get; set; }
        public string QrcFilename { get; set; }
        public string OwnerUuid { get; set; }

        public virtual Event Event { get; set; }
    }
}
