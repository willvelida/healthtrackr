using Healthtrackr.Activity.Common.Models;
using Healthtrackr.Activity.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Healthtrackr.Activity.Functions
{
    public class CreateActivitySummaryRecords
    {
        private readonly IActivityService _activityService;
        private readonly ILogger<CreateActivitySummaryRecords> _logger;

        public CreateActivitySummaryRecords(IActivityService activityService, ILogger<CreateActivitySummaryRecords> logger)
        {
            _activityService = activityService;
            _logger = logger;
        }

        [Function(nameof(CreateActivitySummaryRecords))]
        public async Task Run([CosmosDBTrigger(
            databaseName: "HealthTrackrDB",
            containerName: "Activity",
            Connection = "CosmosDbConnection",
            StartFromBeginning = true,
            LeaseContainerName = "leases",
            LeaseContainerPrefix = "Activity")] IReadOnlyList<ActivityEnvelope> activityEnvelopes)
        {
            try
            {
                if (activityEnvelopes != null && activityEnvelopes.Count > 0)
                {
                    foreach (var activityEnvelope in activityEnvelopes)
                    {
                        _logger.LogInformation($"Attempting to normalize Activity Envelope for {activityEnvelope.Date}");
                        await _activityService.MapAndSaveActivitySummaryRecord(activityEnvelope);
                        _logger.LogInformation($"Activity Summary Record for {activityEnvelope.Date} persisted. Adding specific activities.");
                        await _activityService.MapAndSaveActivityRecords(activityEnvelope);
                        _logger.LogInformation($"Records for {activityEnvelope.Date} saved");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(CreateActivitySummaryRecords)}: {ex.Message}");
                throw;
            }
        }
    }
}
