using Healthtrackr.Activity.Common.Models;
using Healthtrackr.Activity.Services.Interfaces;
using Microsoft.Extensions.Logging;
using res = Healthtrackr.Activity.Common.FitbitResponses;

namespace Healthtrackr.Activity.Services
{
    public class ActivityMappers : IActivityMappers
    {
        private readonly ILogger<ActivityMappers> _logger;

        public ActivityMappers(ILogger<ActivityMappers> logger)
        {
            _logger = logger;
        }

        public ActivityRecord MapActivityToActivityRecord(res.Activity activity)
        {
            try
            {
                _logger.LogInformation("Mapping Activity to Activity record");
                ActivityRecord record = new ActivityRecord
                {
                    ActivityName = activity.name,
                    Calories = activity.calories,
                    Duration = activity.duration,
                    Date = activity.startDate,
                    Time = activity.startTime,
                    Steps = activity.steps
                };
                _logger.LogInformation("Activity mapped");
                return record;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(MapActivityToActivityRecord)}: {ex.Message}");
                throw;
            }
        }

        public ActivitySummaryRecord MapSummaryToActivitySummaryRecord(res.Summary summary)
        {
            try
            {
                _logger.LogInformation("Mapping summary to activity summary record");
                ActivitySummaryRecord activitySummaryRecord = new ActivitySummaryRecord
                {
                    CaloriesBurned = summary.caloriesOut,
                    Steps = summary.steps,
                    Floors = summary.floors,
                    Distance = summary.distances.Where(d => d.activity == "total").Select(d => d.distance).FirstOrDefault(),
                    MinutesSedentary = summary.sedentaryMinutes,
                    MinutesLightlyActive = summary.lightlyActiveMinutes,
                    MinutesFairlyActive = summary.fairlyActiveMinutes,
                    MinutesVeryActive = summary.veryActiveMinutes,
                    ActivityCalories = summary.activityCalories,
                };
                _logger.LogInformation("Summary mapped.");
                return activitySummaryRecord;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(MapSummaryToActivitySummaryRecord)}: {ex.Message}");
                throw;
            }
        }
    }
}
