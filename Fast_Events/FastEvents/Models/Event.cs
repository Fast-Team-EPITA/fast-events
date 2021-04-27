using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastEvents.Models
{
    enum Category
    {

    }
    public class Event : Interfaces.IObjectWithId
    {
        public long id { get; set; }
        string name { get; set; }
        string organizer { get; set; }
        DateTime startDate { get; set; }
        DateTime endTime { get; set; }
        int capacity { get; set; }
        string location { get; set; }
        string description { get; set; }
        string pictureFilename { get; set; }
        string ownerUuid { get; set; }
        Category category { get; set; }
        int nbAvailableTickets { get; set; }

    }
}
