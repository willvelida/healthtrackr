using System.Diagnostics.CodeAnalysis;

namespace Healthtrackr.Activity.Common.FitbitResponses
{
    [ExcludeFromCodeCoverage]
    public class Distance
    {
        public string activity { get; set; }
        public double distance { get; set; }
    }
}
