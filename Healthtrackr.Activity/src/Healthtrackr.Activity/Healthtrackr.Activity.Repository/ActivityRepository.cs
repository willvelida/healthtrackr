using Healthtrackr.Activity.Common.Models;
using Healthtrackr.Activity.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
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

        public async Task AddActivityDistancesRecord(ActivityDistancesRecord activityDistancesRecord)
        {
            try
            {
                _logger.LogInformation($"Attempting to persist Activity Distances record for date: {activityDistancesRecord.Date}");
                _context.ActivityDistances.Add(activityDistancesRecord);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Activity Distances record for data: {activityDistancesRecord.Date} successfully persisted");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(AddActivityDistancesRecord)}: {ex.Message}");
                throw;
            }
        }

        public async Task AddActivityHeartRateZoneRecord(ActivityHeartRateZonesRecord activityHeartRateZonesRecord)
        {
            try
            {
                _logger.LogInformation($"Attempting to persist Activity Heart Rate Zone record for date: {activityHeartRateZonesRecord.Date}");
                _context.ActivityHeartRateZones.Add(activityHeartRateZonesRecord);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Activity Heart Rate Zone record for data: {activityHeartRateZonesRecord.Date} successfully persisted");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(AddActivityHeartRateZoneRecord)}: {ex.Message}");
                throw;
            }
        }

        public async Task AddActivityRecord(ActivityRecord activityRecord)
        {
            try
            {
                _logger.LogInformation("Getting Activity Summary Id");
                var activitySummaryId = _context.ActivitySummary
                    .OrderByDescending(p => p.Id)
                    .Select(p => p.Id)
                    .FirstOrDefault();
                _logger.LogInformation($"Activity Summary Id: {activitySummaryId} retrieved. Setting it in Activity Record");
                activityRecord.ActivitySummaryId = activitySummaryId;

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

        public async Task AddActivitySummaryRecord(ActivitySummaryRecord activitySummaryRecord, int heartRateZoneId, int distanceId)
        {
            try
            {
                _logger.LogInformation($"Activity Heart Rate Zone Id: {heartRateZoneId} retrieved");
                _logger.LogInformation($"Activity Distance Id: {distanceId} retrieved.");
                activitySummaryRecord.ActivityHeartRateZonesId = heartRateZoneId;
                activitySummaryRecord.ActivityDistancesId = distanceId;

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

        public async Task<int> GetActivityDistanceId()
        {
            try
            {
                _logger.LogInformation("Getting Activity Distance Id");
                var activityDistanceId = await _context.ActivityDistances
                    .OrderByDescending(p => p.Id)
                    .Select(p => p.Id)
                    .FirstOrDefaultAsync();
                _logger.LogInformation($"Activity Distance Id: {activityDistanceId} retrieved.");

                return activityDistanceId;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetActivityDistanceId)}: {ex.Message}");
                throw;
            }
        }

        public async Task<int> GetActivityHeartRateZoneId()
        {
            try
            {
                _logger.LogInformation("Getting Activity Heart Rate Zone Id");
                var activityHeartRateZoneId = await _context.ActivityHeartRateZones
                    .OrderByDescending(p => p.Id)
                    .Select(p => p.Id)
                    .FirstOrDefaultAsync();
                _logger.LogInformation($"Activity Heart Rate Zone Id: {activityHeartRateZoneId} retrieved");

                return activityHeartRateZoneId;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetActivityHeartRateZoneId)}: {ex.Message}");
                throw;
            }
        }
    }
}
