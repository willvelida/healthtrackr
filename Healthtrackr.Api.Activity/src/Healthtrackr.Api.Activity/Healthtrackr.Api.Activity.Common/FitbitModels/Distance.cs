using System.Diagnostics.CodeAnalysis;

namespace Healthtrackr.Api.Activity.Common.FitbitModels
{
    [ExcludeFromCodeCoverage]
    public class Distance
    {
        public string activity { get; set; }
        public double distance { get; set; }
    }
}
