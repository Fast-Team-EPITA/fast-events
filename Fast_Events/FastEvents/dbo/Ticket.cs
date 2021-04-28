using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastEvents.dbo
{
    public class Ticket : Interfaces.IObjectWithId
    {
        public long id { get; set; }
        public string name { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public long eventId { get; set; }
        public string ownerUuid { get; set; }
        public string eventName { get; set; }
        public string qrcFilename { get; set; }
    }
}
