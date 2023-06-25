using Healthtrackr.Activity.Common.Models;

namespace Healthtrackr.Activity.Repository.Interfaces
{
    public interface IActivityRepository
    {
        Task AddActivitySummaryRecord(ActivitySummaryRecord activitySummaryRecord);
        Task AddActivityRecord(ActivityRecord activityRecord);
    }
}
