using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastEvents.Models
{
    public class StatModel : Interfaces.IObjectWithId
    {
        public long id { get; set; }
        long eventId { get; set; }
        DateTime date { get; set; }
    }
}
