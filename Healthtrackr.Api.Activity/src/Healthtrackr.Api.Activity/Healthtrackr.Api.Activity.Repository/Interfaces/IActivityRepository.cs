using Healthtrackr.Api.Activity.Common.Models;

namespace Healthtrackr.Api.Activity.Repository.Interfaces
{
    public interface IActivityRepository
    {
        Task<List<ActivityRecord>> GetActivityRecordsByDate(string date);
        Task<ActivitySummaryRecord> GetActivitySummaryRecordByDate(string date);
    }
}
