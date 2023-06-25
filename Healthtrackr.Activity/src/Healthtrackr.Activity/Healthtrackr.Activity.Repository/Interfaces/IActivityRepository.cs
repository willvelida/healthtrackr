using Healthtrackr.Activity.Common.Models;

namespace Healthtrackr.Activity.Repository.Interfaces
{
    public interface IActivityRepository
    {
        Task AddActivityHeartRateZoneRecord(ActivityHeartRateZonesRecord activityHeartRateZonesRecord);
        Task AddActivityDistancesRecord(ActivityDistancesRecord activityDistancesRecord);
        Task AddActivitySummaryRecord(ActivitySummaryRecord activitySummaryRecord, int heartRateZoneId, int distanceId);
        Task AddActivityRecord(ActivityRecord activityRecord);
        Task<int> GetActivityHeartRateZoneId();
        Task<int> GetActivityDistanceId();
    }
}
