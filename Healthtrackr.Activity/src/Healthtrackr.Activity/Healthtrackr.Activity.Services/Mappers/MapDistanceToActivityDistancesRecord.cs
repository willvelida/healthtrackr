using AutoMapper;
using Healthtrackr.Activity.Common.FitbitResponses;
using Healthtrackr.Activity.Common.Models;

namespace Healthtrackr.Activity.Services.Mappers
{
    public class MapDistanceToActivityDistancesRecord : Profile
    {
        public MapDistanceToActivityDistancesRecord()
        {
            CreateMap<Distance, ActivityDistancesRecord>()
                .ForMember(dest => dest.ActivityType,
                    opt => opt.MapFrom(src => src.activity))
                .ForMember(dest => dest.Distance,
                    opt => opt.MapFrom(src => Math.Round(src.distance, 2)));
        }
    }
}
