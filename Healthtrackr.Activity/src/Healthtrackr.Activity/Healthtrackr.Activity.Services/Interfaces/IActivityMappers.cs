using Healthtrackr.Activity.Common.Models;
using res = Healthtrackr.Activity.Common.FitbitResponses;

namespace Healthtrackr.Activity.Services.Interfaces
{
    public interface IActivityMappers
    {
        ActivityRecord MapActivityToActivityRecord(res.Activity activity);
        ActivitySummaryRecord MapSummaryToActivitySummaryRecord(res.Summary summary);
    }
}
