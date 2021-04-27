using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastEvents.Models
{
    public class StatByEventModel : Interfaces.IObjectWithId
    {
        public long id { get; set; }
        public int views { get; set; }
    }
}
