using FastEvents.dbo;

namespace FastEvents.Models
{
    public class DetailViewModel
    {
        public string ImagePath { get; set; }
        public bool IsOwner { get; set; }
        public bool HasTicket { get; set; }
        public string GoogleMapLink { get; set; }
        public EventUi EventUi { get; set; }

        public DetailViewModel(EventUi eventUi, bool isOwner, bool hasTicket)
        {
            EventUi = eventUi;
            IsOwner = isOwner;
            ImagePath = @"..\Resources\Images\" + EventUi.PictureFilename;
            GoogleMapLink = "https://www.google.com/maps/search/?api=1&query=" + EventUi.Location.Replace(" ", "+");
        }
    }
}