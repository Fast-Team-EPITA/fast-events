using FastEvents.dbo;
using System;

namespace FastEvents.Models
{
    public class CreateOrEditViewModel
    {
        public EventUi EventUi { get; set; }

        public bool Create { get; set; }

        public string eventName { get; set; }
        public string organiserName { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endingDate { get; set; }
        public string category { get; set; }
        public int numberPlaces { get; set; }
        public string location { get; set; }
        public string description { get; set; }
        public string image { get; set; }

        public CreateOrEditViewModel(EventUi eventUi, bool create)
        {
            EventUi = eventUi;
            Create = create;
        }
    }
}