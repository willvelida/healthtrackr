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
                .ForMember(dest => dest.CaloriesEstimationMu,
                    opt => opt.MapFrom(src => src.calorieEstimationMu))
                .ForMember(dest => dest.CaloriesBMR,
                    opt => opt.MapFrom(src => src.caloriesBMR))
                .ForMember(dest => dest.CaloriesOut,
                    opt => opt.MapFrom(src => src.caloriesOut))
                .ForMember(dest => dest.ActivityCalories,
                    opt => opt.MapFrom(src => src.activityCalories))
                .ForMember(dest => dest.FairlyActiveMinutes,
                    opt => opt.MapFrom(src => src.fairlyActiveMinutes))
                .ForMember(dest => dest.Floors,
                    opt => opt.MapFrom(src => src.floors))
                .ForMember(dest => dest.LightlyActiveMinutes,
                    opt => opt.MapFrom(src => src.lightlyActiveMinutes))
                .ForMember(dest => dest.MarginalCalories,
                    opt => opt.MapFrom(src => src.marginalCalories))
                .ForMember(dest => dest.RestingHeartRate,
                    opt => opt.MapFrom(src => src.restingHeartRate))
                .ForMember(dest => dest.SedentaryMinutes,
                    opt => opt.MapFrom(src => src.sedentaryMinutes))
                .ForMember(dest => dest.Steps,
                    opt => opt.MapFrom(src => src.steps))
                .ForMember(dest => dest.VeryActiveMinutes,
                    opt => opt.MapFrom(dest => dest.veryActiveMinutes));
        }
    }
}
