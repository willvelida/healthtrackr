using Healthtrackr.Activity.Common.FitbitResponses;
using Newtonsoft.Json;

namespace Healthtrackr.Activity.Common.Models
{
    public class ActivityEnvelope
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public ActivityResponse Activity { get; set; }
        public string Date { get; set; }
        public string DocumentType { get; set; }
    }
}
