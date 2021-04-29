using FastEvents.dbo;

namespace FastEvents.Models
{
    public class CreateOrEditViewModel
    {
        public EventUi EventUi { get; set; }

        public bool Create { get; set; }

        public CreateOrEditViewModel(EventUi eventUi, bool create)
        {
            EventUi = eventUi;
            Create = create;
        }
    }
}