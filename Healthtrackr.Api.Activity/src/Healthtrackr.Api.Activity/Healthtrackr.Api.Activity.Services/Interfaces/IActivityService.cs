using Healthtrackr.Api.Activity.Common.Models;

namespace Healthtrackr.Api.Activity.Services.Interfaces
{
    public interface IActivityService
    {
        Task<ActivityEnvelope> GetActivityEnvelope(string date);
        Task<ActivitySummaryRecord> GetActivitySummary(string date);
        Task<List<ActivityRecord>> GetActivityRecords(string date);
        bool IsDateValid(string date);
    }
}
