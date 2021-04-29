using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastEvents.dbo
{
    public class Stat : Interfaces.IObjectWithId
    {
        public long Id { get; set; }
        public long EventId { get; set; }
        public DateTime Date { get; set; }
    }
}
