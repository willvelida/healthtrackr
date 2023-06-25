using Healthtrackr.Activity.Common.Models;
using Healthtrackr.Activity.Repository.Interfaces;
using Microsoft.Extensions.Logging;

namespace Healthtrackr.Activity.Repository
{
    public class ActivityRepository : IActivityRepository
    {
        private readonly ActivityContext _context;
        private readonly ILogger<ActivityRepository> _logger;

        public ActivityRepository(ActivityContext context, ILogger<ActivityRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddActivityRecord(ActivityRecord activityRecord)
        {
            try
            {
                _logger.LogInformation($"Attempting to persist Activity record for {activityRecord.Date}");
                _context.Activity.Add(activityRecord);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Activity record for {activityRecord.Date} successfully persisted");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(AddActivityRecord)}: {ex.Message}");
                throw;
            }
        }

        public async Task AddActivitySummaryRecord(ActivitySummaryRecord activitySummaryRecord)
        {
            try
            {
                _logger.LogInformation($"Attempting to persist Activity Summary record for {activitySummaryRecord.Date}");
                _context.ActivitySummary.Add(activitySummaryRecord);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Activity Summary record for {activitySummaryRecord.Date} successfully persisted");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(AddActivitySummaryRecord)}: {ex.Message}");
                throw;
            }
        }
    }
}
