using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace FastEvents.DataAccess
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<dbo.Stat, EfModels.Stat>();
            CreateMap<EfModels.Stat, dbo.Stat>();

            CreateMap<dbo.Event, EfModels.Event>();
            CreateMap<EfModels.Event, dbo.Event>();

            CreateMap<dbo.Ticket, EfModels.Ticket>();
            CreateMap<EfModels.Ticket, dbo.Ticket>();

            CreateMap<dbo.StatByEvent, EfModels.EventView>();
            CreateMap<EfModels.EventView, dbo.StatByEvent>();

            CreateMap<dbo.TicketView, EfModels.TicketView>();
            CreateMap<EfModels.TicketView, dbo.TicketView>();

        }
    }
}
