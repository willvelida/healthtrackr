using AutoMapper;
using Healthtrackr.Activity.Common.FitbitResponses;
using Healthtrackr.Activity.Common.Models;

namespace Healthtrackr.Activity.Services.Mappers
{
    public class MapSummaryToActivitySummaryRecord : Profile
    {
        public MapSummaryToActivitySummaryRecord()
        {
            CreateMap<Summary, ActivitySummaryRecord>()
                .ForMember(dest => dest.CaloriesOut,
                    opt => opt.MapFrom(src => src.caloriesOut))
                .ForMember(dest => dest.ActivityCalories,
                    opt => opt.MapFrom(src => src.activityCalories))
                .ForMember(dest => dest.Elevation,
                    opt => opt.MapFrom(src => src.elevation))
                .ForMember(dest => dest.FairlyActiveMinutes,
                    opt => opt.MapFrom(src => src.fairlyActiveMinutes))
                .ForMember(dest => dest.Floors,
                    opt => opt.MapFrom(src => src.floors))
                .ForMember(dest => dest.LightlyActiveMinutes,
                    opt => opt.MapFrom(src => src.lightlyActiveMinutes))
                .ForMember(dest => dest.SedentaryMinutes,
                    opt => opt.MapFrom(src => src.sedentaryMinutes))
                .ForMember(dest => dest.Steps,
                    opt => opt.MapFrom(src => src.steps))
                .ForMember(dest => dest.VeryActiveMinutes,
                    opt => opt.MapFrom(dest => dest.veryActiveMinutes));
        }
    }
}
