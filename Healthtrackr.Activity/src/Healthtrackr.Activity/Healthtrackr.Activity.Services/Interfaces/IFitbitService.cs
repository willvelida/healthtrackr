using Healthtrackr.Activity.Common.FitbitResponses;

namespace Healthtrackr.Activity.Services.Interfaces
{
    public interface IFitbitService
    {
        Task<ActivityResponse> GetActivityResponse(string date);
    }
}
