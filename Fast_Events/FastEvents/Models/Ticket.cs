using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastEvents.Models
{
    public class Ticket : Interfaces.IObjectWithId
    {
        public long id { get; set; }
        long eventId { get; set; }
        string ownerUuid { get; set; }
        string qrcFilename { get; set; }
    }
}
