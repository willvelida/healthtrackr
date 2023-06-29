using Healthtrackr.Api.Activity.Common.Models;
using Healthtrackr.Api.Activity.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Healthtrackr.Api.Activity.Repository
{
    public class ActivityRepository : IActivityRepository
    {
        private readonly ActivityContext _activityContext;
        private readonly ILogger<ActivityRepository> _logger;

        public ActivityRepository(ActivityContext activityContext, ILogger<ActivityRepository> logger)
        {
            _activityContext = activityContext;
            _logger = logger;
        }

        public async Task<List<ActivityRecord>> GetActivityRecordsByDate(string date)
        {
            try
            {
                return await _activityContext.Activity.Where(a => a.Date == DateTime.Parse(date)).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetActivityRecordsByDate)}: {ex.Message}");
                throw;
            }
        }

        public async Task<ActivitySummaryRecord> GetActivitySummaryRecordByDate(string date)
        {
            try
            {
                return await _activityContext.ActivitySummary.FirstOrDefaultAsync(a => a.Date == DateTime.Parse(date));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetActivitySummaryRecordByDate)}: {ex.Message}");
                throw;
            }
        }
    }
}
