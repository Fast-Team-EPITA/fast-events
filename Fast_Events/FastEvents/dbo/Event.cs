using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastEvents.dbo
{
    public enum Category
    {
        Concert,
        Conference,
        OpenAir
    }
    
    public class Event: Interfaces.IObjectWithId
    {
        public long id { get; set; }
        public string name { get; set; }
        public string organizer { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public int capacity { get; set; }
        public string location { get; set; }
        public string description { get; set; }
        public string pictureFilename { get; set; }
        public string ownerUuid { get; set; }
        public Category category { get; set; }
        public int nbAvailableTickets { get; set; }

    }
}
