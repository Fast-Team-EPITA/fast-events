namespace FastEvents.Models
{
    public class CreateOrEditViewModel
    {
        public Event Event { get; set; }

        public bool Create { get; set; }

        public CreateOrEditViewModel(Event @event, bool create)
        {
            Event = @event;
            Create = create;
        }
    }
}