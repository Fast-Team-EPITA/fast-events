using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastEvents.Models
{
    public class Stat : Interfaces.IObjectWithId
    {
        public long id { get; set; }
        public long eventId { get; set; }
        public DateTime date { get; set; }
    }
}
