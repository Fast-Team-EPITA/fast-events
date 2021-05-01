using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastEvents.dbo
{
    public class Event: Interfaces.IObjectWithId
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Organizer { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Capacity { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string PictureFilename { get; set; }
        public string OwnerUuid { get; set; }
        public Category Category { get; set; }

        public Event() {}
        
        public Event(EventUi eventUi)
        {
            Id = eventUi.Id;
            Name = eventUi.Name ?? "";
            Organizer = eventUi.Organizer ?? "";
            StartDate = eventUi.StartDate;
            EndDate = eventUi.EndDate;
            Capacity = eventUi.Capacity;
            Location = eventUi.Location ?? "";
            Description = eventUi.Description ?? "";
            PictureFilename = eventUi.PictureFilename ?? "event_place_holder.jpg";
            OwnerUuid = eventUi.OwnerUuid ?? "";
            Category = eventUi.Category;
        } 

    }
}
