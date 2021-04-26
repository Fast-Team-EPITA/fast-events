using System;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Fast_Events.Views.Home
{
    public class EventDetailModel : PageModel
    {
        public String imagePath { get; set; }
        public Boolean isOwner { get; set; }
        public String location { get; set; }
        public String googleMapLink { get; set; }

        EventDetailModel()
        {
        }
        
        public void OnGet()
        {
            imagePath = @"..\Resources\Images\" + "event_place_holder.jpg";
            isOwner = true;
            location = "1 Rue Voltaire, 94270, Le Kremlin Bicetre".Replace(" ", "+");
            googleMapLink = "https://www.google.com/maps/search/?api=1&query=" + location;
        }
    }
}