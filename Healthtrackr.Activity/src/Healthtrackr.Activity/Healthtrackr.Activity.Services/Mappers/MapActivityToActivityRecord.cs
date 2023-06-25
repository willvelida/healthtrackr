using AutoMapper;
using Healthtrackr.Activity.Common.Models;
using res = Healthtrackr.Activity.Common.FitbitResponses;

namespace Healthtrackr.Activity.Services.Mappers
{
    public class MapActivityToActivityRecord : Profile
    {
        public MapActivityToActivityRecord()
        {
            CreateMap<res.Activity, ActivityRecord>()
                .ForMember(dest => dest.ActivityName,
                    opt => opt.MapFrom(src => src.name))
                .ForMember(dest => dest.Calories,
                    opt => opt.MapFrom(src => src.calories))
                .ForMember(dest => dest.Duration,
                    opt => opt.MapFrom(src => src.duration))
                .ForMember(dest => dest.Date,
                    opt => opt.MapFrom(src => src.startDate))
                .ForMember(dest => dest.Time,
                    opt => opt.MapFrom(src => src.startTime))
                .ForMember(dest => dest.Steps,
                    opt => opt.MapFrom(src => src.steps));
        }
    }
}
