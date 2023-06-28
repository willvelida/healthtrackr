using System.Diagnostics.CodeAnalysis;

namespace Healthtrackr.Api.Activity.Common.FitbitModels
{
    [ExcludeFromCodeCoverage]
    public class ActivityResponse
    {
        public List<Activity> activities { get; set; }
        public Goals goals { get; set; }
        public Summary summary { get; set; }
    }
}
