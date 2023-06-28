using Healthtrackr.Api.Activity.Common.Models;
using Healthtrackr.Api.Activity.Repository.Interfaces;
using Healthtrackr.Api.Activity.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Healthtrackr.Api.Activity.Services
{
    public class ActivityService : IActivityService
    {
        private readonly ICosmosDbRepository _cosmosDbRepository;
        private readonly ILogger<ActivityService> _logger;

        public ActivityService(ICosmosDbRepository cosmosDbRepository, ILogger<ActivityService> logger)
        {
            _cosmosDbRepository = cosmosDbRepository;
            _logger = logger;
        }

        public async Task<ActivityEnvelope> GetActivityEnvelope(string date)
        {
            try
            {
                var activityEnvelopes = await _cosmosDbRepository.GetActivityEnvelopeByDate(date);

                return activityEnvelopes.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetActivityEnvelope)}: {ex.Message}");
                throw;
            }
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
    }
}