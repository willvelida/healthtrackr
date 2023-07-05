using Healthtrackr.Api.Activity.Common.Filters;
using Healthtrackr.Api.Activity.Common.Models;
using Healthtrackr.Api.Activity.Common.Wrappers;

namespace Healthtrackr.Api.Activity.Services.Interfaces
{
    public interface IActivityService
    {
        Task<ActivityEnvelope> GetActivityEnvelope(string date);
        Task<ActivitySummaryRecord> GetActivitySummary(string date);
        Task<List<ActivityRecord>> GetActivityRecords(string date);
        Task<PagedResponse<List<ActivityRecord>>> GetAllActivityRecords(PaginationFilter paginationFilter, string route);
        Task<PagedResponse<List<ActivitySummaryRecord>>> GetAllActivitySummaryRecords(PaginationFilter paginationFilter, string route);
        bool IsDateValid(string date);
    }
}
