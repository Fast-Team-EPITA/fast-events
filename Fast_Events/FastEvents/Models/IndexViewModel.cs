using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastEvents.dbo;

namespace FastEvents.Models
{
    public class IndexViewModel
    {
        public List<EventUi> EventUis { get; set; }

        public IndexViewModel(List<EventUi> eventUis)
        {
            EventUis = eventUis;
        }

    }
}
