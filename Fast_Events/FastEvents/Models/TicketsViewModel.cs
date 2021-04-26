using System;
using System.Collections.Generic;

namespace FastEvents.Models
{
    public class TicketsViewModel
    {
        public List<string> Tickets { get; set; }

        public TicketsViewModel()
        {
            Tickets = new List<string>();
            Tickets.Add("");
            Tickets.Add("");
            Tickets.Add("");
        }
    }
}