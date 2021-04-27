using System;
using System.Collections.Generic;

#nullable disable

namespace FastEvents.DataAccess.EfModels
{
    public partial class TicketView
    {
        public string Name { get; set; }
        public long Id { get; set; }
        public long EventId { get; set; }
        public string QrcFilename { get; set; }
        public string OwnerUuid { get; set; }
    }
}
