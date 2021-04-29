using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastEvents.dbo
{
    public class Ticket : Interfaces.IObjectWithId
    {
        public long Id { get; set; }
        public long EventId { get; set; }
        public string OwnerUuid { get; set; }
        public string EventName { get; set; }
        public string QrcFilename { get; set; }

        public virtual EventUi EventUi { get; set; }
    }
}
