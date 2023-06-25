using Healthtrackr.Activity.Common.Models;
using Healthtrackr.Activity.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Healthtrackr.Activity.Functions
{
    public class CreateSummaryRecords
    {
        private readonly IActivityService _activityService;
        private readonly ILogger<CreateSummaryRecords> _logger;

        public CreateSummaryRecords(IActivityService activityService, ILogger<CreateSummaryRecords> logger)
        {
            _activityService = activityService;
            _logger = logger;
        }

        [Function(nameof(CreateSummaryRecords))]
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
                _logger.LogError($"Exception thrown in {nameof(CreateSummaryRecords)}: {ex.Message}");
                throw;
            }
        }
    }
}
