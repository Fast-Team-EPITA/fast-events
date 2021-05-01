using FastEvents.dbo;

namespace FastEvents.Models
{
    public class DetailViewModel
    {
        public bool IsOwner { get; set; }
        public bool HasTicket { get; set; }
        public EventUi EventUi { get; set; }
    }
}