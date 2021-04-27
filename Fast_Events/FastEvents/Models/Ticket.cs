using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastEvents.Models
{
    public class Ticket : Interfaces.IObjectWithId
    {
        public long id { get; set; }
        public long eventId { get; set; }
        public string ownerUuid { get; set; }
        public string qrcFilename { get; set; }
    }
}
