using Healthtrackr.Api.Activity.Common.FitbitModels;

namespace Healthtrackr.Api.Activity.Common.Models
{
    public class ActivityEnvelope
    {
        public string Id { get; set; }
        public ActivityResponse Activity { get; set; }
        public string Date { get; set; }
        public string DocumentType { get; set; }
    }
}
