using Healthtrackr.Activity.Common.FitbitResponses;
using Healthtrackr.Activity.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Healthtrackr.Activity.Functions
{
    public class CreateActivityEnvelope
    {
        private readonly IActivityService _activityService;
        private readonly ILogger<CreateActivityEnvelope> _logger;

        public CreateActivityEnvelope(IActivityService activityService, ILogger<CreateActivityEnvelope> logger)
        {
            _activityService = activityService;
            _logger = logger;
        }

        [Function(nameof(CreateActivityEnvelope))]
        public async Task RunAsync([ServiceBusTrigger("activityqueue", Connection = "ServiceBusConnection")] string activityQueueItem)
        {
            try
            {
                var date = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                _logger.LogInformation($"Attempting to parse Activity Response message for {date}");

                var activity = JsonConvert.DeserializeObject<ActivityResponse>(activityQueueItem);

                _logger.LogInformation($"Adding activity for {date} to database.");
                await _activityService.MapActivityEnvelopeAndSaveToDatabase(date, activity);
                _logger.LogInformation($"Activity for {date} successfully persisted");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(CreateActivityEnvelope)}: {ex.Message}");
                throw;
            }
        }
    }
}
