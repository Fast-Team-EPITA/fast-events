using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastEvents.dbo
{
    public class TicketView : Interfaces.IObjectWithId
    {
        public string Name { get; set; }
        public long id { get; set; }
        public long EventId { get; set; }
        public string QrcFilename { get; set; }
        public string OwnerUuid { get; set; }
    }
}
