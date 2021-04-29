
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastEvents.dbo.Interfaces
{
    public interface IObjectWithId
    {
        long Id { get; set; }
    }
}
