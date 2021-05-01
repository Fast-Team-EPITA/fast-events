using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastEvents.dbo
{
    public class StatByEvent : Interfaces.IObjectWithId
    {
        public int NumberView { get; set; }
        public string Name { get; set; }
        public long Id { get; set; }
    }
}
