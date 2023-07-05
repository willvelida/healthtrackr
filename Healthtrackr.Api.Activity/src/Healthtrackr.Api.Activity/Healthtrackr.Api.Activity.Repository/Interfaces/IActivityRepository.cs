using Healthtrackr.Api.Activity.Common;
using Healthtrackr.Api.Activity.Common.Filters;
using Healthtrackr.Api.Activity.Common.Models;

namespace Healthtrackr.Api.Activity.Repository.Interfaces
{
    public interface IActivityRepository
    {
        Task<List<ActivityRecord>> GetActivityRecordsByDate(string date);
        Task<ActivitySummaryRecord> GetActivitySummaryRecordByDate(string date);
        Task<List<ActivityRecord>> GetActivityRecords(PaginationFilter paginationFilter);
        Task<int> GetActivityRecordsCount();
        Task<List<ActivitySummaryRecord>> GetActivitySummaryRecords(PaginationFilter paginationFilter);
        Task<int> GetActivitySummaryRecordCount();
    }
}
