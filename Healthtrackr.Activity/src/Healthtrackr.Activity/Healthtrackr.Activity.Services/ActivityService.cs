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
        private readonly ICosmosDbRepository _cosmosDbRepository;
        private readonly ILogger<ActivityService> _logger;

        public ActivityService(ServiceBusClient serviceBusClient, ICosmosDbRepository cosmosDbRepository, ILogger<ActivityService> logger)
        {
            _serviceBusClient = serviceBusClient;
            _cosmosDbRepository = cosmosDbRepository;
            _logger = logger;
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
