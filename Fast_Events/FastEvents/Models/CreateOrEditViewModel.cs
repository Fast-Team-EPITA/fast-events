using FastEvents.dbo;

namespace FastEvents.Models
{
    public class CreateOrEditViewModel
    {
        public Event Event { get; set; }

        public bool Create { get; set; }

        public CreateOrEditViewModel(Event selectedEvent, bool create)
        {
            Event = selectedEvent;
            Create = create;
        }
    }
}