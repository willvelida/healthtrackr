using AutoMapper;
using Healthtrackr.Activity.Common.FitbitResponses;
using Healthtrackr.Activity.Common.Models;

namespace Healthtrackr.Activity.Services.Mappers
{
    public class MapHeartRateZoneToActivityHeartRateZonesRecord : Profile
    {
        public MapHeartRateZoneToActivityHeartRateZonesRecord()
        {
            CreateMap<HeartRateZone, ActivityHeartRateZonesRecord>()
                .ForMember(dest => dest.Name,
                    opt => opt.MapFrom(src => src.name))
                .ForMember(dest => dest.Minutes,
                    opt => opt.MapFrom(src => src.minutes))
                .ForMember(dest => dest.MaxHR,
                    opt => opt.MapFrom(src => src.max))
                .ForMember(dest => dest.MinHR,
                    opt => opt.MapFrom(src => src.min))
                .ForMember(dest => dest.CaloriesOut,
                    opt => opt.MapFrom(src => Math.Round(src.caloriesOut, 2)));
        }
    }
}
