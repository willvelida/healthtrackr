using Azure;
using Healthtrackr.Api.Activity.Common.Filters;
using Healthtrackr.Api.Activity.Common.Models;
using Healthtrackr.Api.Activity.Common.Wrappers;
using Healthtrackr.Api.Activity.Repository.Interfaces;
using Healthtrackr.Api.Activity.Services.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace Healthtrackr.Api.Activity.Services
{
    public class ActivityService : IActivityService
    {
        private readonly ICosmosDbRepository _cosmosDbRepository;
        private readonly IActivityRepository _activityRepository;
        private readonly IUriService _uriService;
        private readonly ILogger<ActivityService> _logger;

        public ActivityService(ICosmosDbRepository cosmosDbRepository, IActivityRepository activityRepository, IUriService uriService, ILogger<ActivityService> logger)
        {
            _cosmosDbRepository = cosmosDbRepository;
            _activityRepository = activityRepository;
            _uriService = uriService;
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

        public async Task<PagedResponse<List<ActivityRecord>>> GetAllActivityRecords(PaginationFilter paginationFilter, string route)
        {
            try
            {
                var activityRecords = await _activityRepository.GetActivityRecords(paginationFilter);
                var totalRecords = await _activityRepository.GetActivityRecordsCount();
                var response = new PagedResponse<List<ActivityRecord>>(activityRecords, paginationFilter.PageNumber, paginationFilter.PageSize);
                var totalPages = ((double)totalRecords / (double)paginationFilter.PageSize);
                int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));
                response.NextPage =
                    paginationFilter.PageNumber >= 1 && paginationFilter.PageNumber < roundedTotalPages
                    ? _uriService.GetPageUri(new PaginationFilter(paginationFilter.PageNumber + 1, paginationFilter.PageSize), route)
                    : null;
                response.PreviousPage =
                    paginationFilter.PageNumber - 1 >= 1 && paginationFilter.PageNumber <= roundedTotalPages
                    ? _uriService.GetPageUri(new PaginationFilter(paginationFilter.PageNumber - 1, paginationFilter.PageSize), route)
                    : null;
                response.FirstPage = _uriService.GetPageUri(new PaginationFilter(1, paginationFilter.PageSize), route);
                response.LastPage = _uriService.GetPageUri(new PaginationFilter(roundedTotalPages, paginationFilter.PageSize), route);
                response.TotalPages = roundedTotalPages;
                response.TotalRecords = totalRecords;
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetAllActivityRecords)}: {ex.Message}");
                throw;
            }
        }

        public async Task<PagedResponse<List<ActivitySummaryRecord>>> GetAllActivitySummaryRecords(PaginationFilter paginationFilter, string route)
        {
            try
            {
                var activitySummaryRecords = await _activityRepository.GetActivitySummaryRecords(paginationFilter);
                var totalRecords = await _activityRepository.GetActivitySummaryRecordCount();
                var response = new PagedResponse<List<ActivitySummaryRecord>>(activitySummaryRecords, paginationFilter.PageNumber, paginationFilter.PageSize);
                var totalPages = ((double)totalRecords / (double)paginationFilter.PageSize);
                int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));
                response.NextPage =
                    paginationFilter.PageNumber >= 1 && paginationFilter.PageNumber < roundedTotalPages
                    ? _uriService.GetPageUri(new PaginationFilter(paginationFilter.PageNumber + 1, paginationFilter.PageSize), route)
                    : null;
                response.PreviousPage =
                    paginationFilter.PageNumber - 1 >= 1 && paginationFilter.PageNumber <= roundedTotalPages
                    ? _uriService.GetPageUri(new PaginationFilter(paginationFilter.PageNumber - 1, paginationFilter.PageSize), route)
                    : null;
                response.FirstPage = _uriService.GetPageUri(new PaginationFilter(1, paginationFilter.PageSize), route);
                response.LastPage = _uriService.GetPageUri(new PaginationFilter(roundedTotalPages, paginationFilter.PageSize), route);
                response.TotalPages = roundedTotalPages;
                response.TotalRecords = totalRecords;
                return response;
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