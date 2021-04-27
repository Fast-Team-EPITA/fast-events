using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastEvents.Models.Interfaces
{
    interface IObjectWithId
    {
        long id { get; set; }
    }
}
