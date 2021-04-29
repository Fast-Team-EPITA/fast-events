using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastEvents.dbo;

namespace FastEvents.Models
{
    public class IndexViewModel
    {
        public enum Category
        {
            Concert,
            Conference,
            OpenAir
        }

        public List<EventUi> EventUis { get; set; }

        public IndexViewModel(List<EventUi> eventUis)
        {
            EventUis = eventUis;
        }

    }
}
