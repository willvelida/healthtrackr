using Healthtrackr.Api.Activity.Common.Models;
using Healthtrackr.Api.Activity.Common.Paging;
using Healthtrackr.Api.Activity.Common.RequestFilters;

namespace Healthtrackr.Api.Activity.Repository.Interfaces
{
    public interface IActivityRepository
    {
        Task<List<ActivityRecord>> GetActivityRecordsByDate(string date);
        Task<ActivitySummaryRecord> GetActivitySummaryRecordByDate(string date);
        Task<PagedList<ActivityRecord>> GetActivityRecords(ActivityParameters activityParameters);
        Task<int> GetActivityRecordsCount();
        Task<PagedList<ActivitySummaryRecord>> GetActivitySummaryRecords(ActivityParameters activityParameters);
        Task<int> GetActivitySummaryRecordCount();
    }
}
