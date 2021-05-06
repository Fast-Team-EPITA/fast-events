using System;
using FastEvents.dbo.Interfaces;

#nullable disable

namespace FastEvents.DataAccess.EfModels
{
    public partial class EventView: IObjectWithId
    {
        public int NumberTickets { get; set; }
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
        public string Category { get; set; }
    }
}
