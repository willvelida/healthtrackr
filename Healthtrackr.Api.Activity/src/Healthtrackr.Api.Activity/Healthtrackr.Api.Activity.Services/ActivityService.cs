using Healthtrackr.Api.Activity.Common.Models;
using Healthtrackr.Api.Activity.Common.Paging;
using Healthtrackr.Api.Activity.Common.RequestFilters;
using Healthtrackr.Api.Activity.Repository.Interfaces;
using Healthtrackr.Api.Activity.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Healthtrackr.Api.Activity.Services
{
    public class ActivityService : IActivityService
    {
        private readonly ICosmosDbRepository _cosmosDbRepository;
        private readonly IActivityRepository _activityRepository;
        private readonly ILogger<ActivityService> _logger;

        public ActivityService(ICosmosDbRepository cosmosDbRepository, IActivityRepository activityRepository, ILogger<ActivityService> logger)
        {
            _cosmosDbRepository = cosmosDbRepository;
            _activityRepository = activityRepository;
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

        public async Task<List<ActivityRecord>> GetActivityRecords(string date)
        {
            try
            {
                return await _activityRepository.GetActivityRecordsByDate(date);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetActivityRecords)}: {ex.Message}");
                throw;
            }
        }

        public async Task<ActivitySummaryRecord> GetActivitySummary(string date)
        {
            try
            {
                return await _activityRepository.GetActivitySummaryRecordByDate(date);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetActivitySummary)}: {ex.Message}");
                throw;
            }
        }

        public async Task<PagedList<ActivityRecord>> GetAllActivityRecords(ActivityParameters activityParameters)
        {
            try
            {
                return await _activityRepository.GetActivityRecords(activityParameters);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetAllActivityRecords)}: {ex.Message}");
                throw;
            }
        }

        public async Task<PagedList<ActivitySummaryRecord>> GetAllActivitySummaryRecords(ActivityParameters activityParameters)
        {
            try
            {
                return await _activityRepository.GetActivitySummaryRecords(activityParameters);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetAllActivitySummaryRecords)}: {ex.Message}");
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