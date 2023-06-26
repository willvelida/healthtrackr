using Azure.Messaging.ServiceBus;
using Healthtrackr.Activity.Common.Models;
using Healthtrackr.Activity.Repository.Interfaces;
using Healthtrackr.Activity.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using res = Healthtrackr.Activity.Common.FitbitResponses;

namespace Healthtrackr.Activity.Services
{
    public class ActivityService : IActivityService
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly IActivityMappers _mapper;
        private readonly IActivityRepository _activityRepository;
        private readonly ICosmosDbRepository _cosmosDbRepository;
        private readonly ILogger<ActivityService> _logger;

        public ActivityService(ServiceBusClient serviceBusClient, ICosmosDbRepository cosmosDbRepository, ILogger<ActivityService> logger, IActivityMappers mapper, IActivityRepository activityRepository)
        {
            _serviceBusClient = serviceBusClient;
            _cosmosDbRepository = cosmosDbRepository;
            _logger = logger;
            _mapper = mapper;
            _activityRepository = activityRepository;
        }

        public bool IsDateValid(string date)
        {
            bool isDateValid = false;
            string pattern = "yyyy-MM-dd";
            DateTime parsedActivityDate;

            if (DateTime.TryParseExact(date, pattern, null, System.Globalization.DateTimeStyles.None, out parsedActivityDate))
            {
                isDateValid = true;
            }

            return isDateValid;
        }

        public async Task MapActivityEnvelopeAndSaveToDatabase(string date, res.ActivityResponse activityResponse)
        {
            try
            {
                ActivityEnvelope activityEnvelope = new ActivityEnvelope
                {
                    Id = Guid.NewGuid().ToString(),
                    Activity = activityResponse,
                    DocumentType = "Activity",
                    Date = date
                };

                await _cosmosDbRepository.CreateActivityDocument(activityEnvelope);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(MapActivityEnvelopeAndSaveToDatabase)}: {ex.Message}");
                throw;
            }
        }

        public async Task MapAndSaveActivityRecords(ActivityEnvelope activityEnvelope)
        {
            try
            {
                var activities = activityEnvelope.Activity.activities;
                if (activities is null)
                {
                    _logger.LogInformation($"No Activities to map. Exiting.");
                    return;
                }

                foreach (var activity in activities)
                {
                    var activityRecord = _mapper.MapActivityToActivityRecord(activity);
                    activityRecord.Date = activityEnvelope.Date;
                    await _activityRepository.AddActivityRecord(activityRecord);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(MapAndSaveActivityRecords)}: {ex.Message}");
                throw;
            }
        }

        public async Task MapAndSaveActivitySummaryRecord(ActivityEnvelope activityEnvelope)
        {
            try
            {
                var activitySummary = activityEnvelope.Activity.summary;
                if (activitySummary is null)
                {
                    _logger.LogInformation($"No Activity Summaries to map. Exiting.");
                    return;
                }

                var activitySummaryRecord = new ActivitySummaryRecord();
                activitySummaryRecord = _mapper.MapSummaryToActivitySummaryRecord(activitySummary);
                activitySummaryRecord.Date = activityEnvelope.Date;

                await _activityRepository.AddActivitySummaryRecord(activitySummaryRecord);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(MapAndSaveActivitySummaryRecord)}: {ex.Message}");
                throw;
            }
        }

        public async Task SendRecordToQueue<T>(T record, string queueName)
        {
            try
            {
                ServiceBusSender serviceBusSender = _serviceBusClient.CreateSender(queueName);
                var messageAsJson = JsonConvert.SerializeObject(record);
                await serviceBusSender.SendMessageAsync(new ServiceBusMessage(messageAsJson));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(SendRecordToQueue)}: {ex.Message}");
                throw;
            }
        }
    }
}
