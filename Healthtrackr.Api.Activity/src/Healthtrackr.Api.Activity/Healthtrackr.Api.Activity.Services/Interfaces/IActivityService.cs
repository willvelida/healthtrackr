using Healthtrackr.Api.Activity.Common.Models;
using Healthtrackr.Api.Activity.Common.Paging;
using Healthtrackr.Api.Activity.Common.RequestFilters;

namespace Healthtrackr.Api.Activity.Services.Interfaces
{
    public interface IActivityService
    {
        Task<ActivityEnvelope> GetActivityEnvelope(string date);
        Task<ActivitySummaryRecord> GetActivitySummary(string date);
        Task<List<ActivityRecord>> GetActivityRecords(string date);
        Task<PagedList<ActivityRecord>> GetAllActivityRecords(ActivityParameters activityParameters);
        Task<PagedList<ActivitySummaryRecord>> GetAllActivitySummaryRecords(ActivityParameters activityParameters);
        bool IsDateValid(string date);
    }
}
