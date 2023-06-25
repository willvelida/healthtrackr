using AutoMapper;
using Healthtrackr.Activity.Common.Models;
using res = Healthtrackr.Activity.Common.FitbitResponses;

namespace Healthtrackr.Activity.Services.Mappers
{
    public class MapSummaryToActivitySummaryRecord : Profile
    {
        public MapSummaryToActivitySummaryRecord()
        {
            CreateMap<res.Summary, ActivitySummaryRecord>()
                .ForMember(dest => dest.CaloriesBurned,
                    opt => opt.MapFrom(src => src.caloriesOut))
                .ForMember(dest => dest.Steps,
                    opt => opt.MapFrom(src => src.steps))
                .ForMember(dest => dest.Distance,
                    opt => opt.MapFrom(src => src.distances.Where(d => d.activity == "total").Select(d => d.distance).FirstOrDefault()))
                .ForMember(dest => dest.Floors,
                    opt => opt.MapFrom(src => src.floors))
                .ForMember(dest => dest.MinutesSedentary,
                    opt => opt.MapFrom(src => src.sedentaryMinutes))
                .ForMember(dest => dest.MinutesLightlyActive,
                    opt => opt.MapFrom(src => src.lightlyActiveMinutes))
                .ForMember(dest => dest.MinutesFairlyActive,
                    opt => opt.MapFrom(src => src.fairlyActiveMinutes))
                .ForMember(dest => dest.MinutesVeryActive,
                    opt => opt.MapFrom(src => src.veryActiveMinutes))
                .ForMember(dest => dest.ActivityCalories,
                    opt => opt.MapFrom(src => src.activityCalories));
        }
    }
}
