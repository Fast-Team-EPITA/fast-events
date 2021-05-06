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
        public int PageNumber { get; set; }
        public Category? SortCategory { get; set; }
        public string SortType { get; set; }
        public bool OwnedEvents { get; set; }
        public string SearchPattern { get; set; }
    }
}
