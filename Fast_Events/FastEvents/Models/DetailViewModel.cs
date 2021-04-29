using FastEvents.dbo;

namespace FastEvents.Models
{
    public class DetailViewModel
    {
        public string ImagePath { get; set; }
        public bool IsOwner { get; set; }
        public string GoogleMapLink { get; set; }
        public Event Event { get; set; }

        public DetailViewModel(Event @event, bool isOwner)
        {
            Event = @event;
            IsOwner = isOwner;
            ImagePath = @"..\Resources\Images\" + Event.pictureFilename;
            GoogleMapLink = "https://www.google.com/maps/search/?api=1&query=" + Event.location.Replace(" ", "+");
        }
    }
}