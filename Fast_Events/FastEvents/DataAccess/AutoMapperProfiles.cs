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

            CreateMap<dbo.EventUi, EfModels.EventView>()
                 .ForMember(d => d.Category, o => o.MapFrom(y => y.Category));
            CreateMap<EfModels.EventView, dbo.EventUi>()
                .ForMember(d => d.Category, o => o.MapFrom(y => y.Category)); ;

            CreateMap<dbo.EventUi, EfModels.EventView>();
            CreateMap<EfModels.EventView, dbo.EventUi>();

            CreateMap<dbo.Ticket, EfModels.Ticket>();
            CreateMap<EfModels.Ticket, dbo.Ticket>();

            CreateMap<dbo.StatByEvent, EfModels.StatByEvent>();
            CreateMap<EfModels.StatByEvent, dbo.StatByEvent>();

            
                
        }
    }
}
