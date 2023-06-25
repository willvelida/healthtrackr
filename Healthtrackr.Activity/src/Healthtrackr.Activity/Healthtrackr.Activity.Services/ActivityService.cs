using AutoMapper;
using Azure.Messaging.ServiceBus;
using Healthtrackr.Activity.Common.FitbitResponses;
using Healthtrackr.Activity.Common.Models;
using Healthtrackr.Activity.Repository.Interfaces;
using Healthtrackr.Activity.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Healthtrackr.Activity.Services
{
    public class ActivityService : IActivityService
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly IMapper _mapper;
        private readonly IActivityRepository _activityRepository;
        private readonly ICosmosDbRepository _cosmosDbRepository;
        private readonly ILogger<ActivityService> _logger;

        public ActivityService(ServiceBusClient serviceBusClient, ICosmosDbRepository cosmosDbRepository, ILogger<ActivityService> logger, IMapper mapper, IActivityRepository activityRepository)
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

        public async Task MapActivityEnvelopeAndSaveToDatabase(string date, ActivityResponse activityResponse)
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

        public async Task MapAndSaveActivityDistanceRecord(ActivityEnvelope activityEnvelope)
        {
            try
            {
                var distances = activityEnvelope.Activity.summary.distances;
                if (distances is null)
                {
                    _logger.LogInformation($"No Activity Distances to map. Exiting.");
                    return;
                }

                foreach (var distance in distances)
                {
                    var activityDistance = new ActivityDistancesRecord();
                    _mapper.Map(distance, activityDistance);
                    activityDistance.Date = activityEnvelope.Date;
                    await _activityRepository.AddActivityDistancesRecord(activityDistance);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(MapAndSaveActivityDistanceRecord)}: {ex.Message}");
                throw;
            }
        }

        public async Task MapAndSaveActivityHeartRateRecord(ActivityEnvelope activityEnvelope)
        {
            try
            {
                var heartRateZones = activityEnvelope.Activity.summary.heartRateZones;
                if (heartRateZones is null)
                {
                    _logger.LogInformation($"No Heart Rate Zones to map. Exiting.");
                    return;
                }

                foreach (var heartRateZone in heartRateZones)
                {
                    var heartRateRecord = new ActivityHeartRateZonesRecord();
                    _mapper.Map(heartRateZone, heartRateRecord);
                    heartRateRecord.Date = activityEnvelope.Date;
                    await _activityRepository.AddActivityHeartRateZoneRecord(heartRateRecord);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(MapAndSaveActivityHeartRateRecord)}: {ex.Message}");
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
                    var activityRecord = new ActivityRecord();
                    _mapper.Map(activity, activityRecord);
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
                _mapper.Map(activitySummary, activitySummaryRecord);
                activitySummaryRecord.Date = activityEnvelope.Date;
                var heartRateId = await _activityRepository.GetActivityHeartRateZoneId();
                var distanceId = await _activityRepository.GetActivityDistanceId();

                await _activityRepository.AddActivitySummaryRecord(activitySummaryRecord, heartRateId, distanceId);
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
