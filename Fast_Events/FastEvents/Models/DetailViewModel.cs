namespace FastEvents.Models
{
    public class DetailViewModel
    {
        public string ImagePath { get; set; }
        public bool IsOwner { get; set; }
        private string Location { get; set; }
        public string GoogleMapLink { get; set; }

        public string EventId { get; set; }

        public DetailViewModel()
        {
            IsOwner = true;
            ImagePath = @"..\Resources\Images\" + "event_place_holder.jpg";
            Location = "1 Rue Voltaire, 94270, Le Kremlin Bicetre".Replace(" ", "+");
            GoogleMapLink = "https://www.google.com/maps/search/?api=1&query=" + Location;
        }
    }
}