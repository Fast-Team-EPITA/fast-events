using System;
using System.Collections.Generic;
using FastEvents.dbo;

namespace FastEvents.Models
{
    public class TicketsViewModel
    {
        public List<Ticket> Tickets { get; set; }

        public TicketsViewModel(List<Ticket> tickets)
        {
            Tickets = tickets;
        }
    }
}