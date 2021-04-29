using System;

#nullable disable

namespace FastEvents.DataAccess.EfModels
{
    public partial class Stat
    {
        public long Id { get; set; }
        public long EventId { get; set; }
        public DateTime Date { get; set; }

        public virtual Event Event { get; set; }
    }
}
