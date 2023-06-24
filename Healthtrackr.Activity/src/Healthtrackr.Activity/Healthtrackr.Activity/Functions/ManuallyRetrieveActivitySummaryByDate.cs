using Healthtrackr.Activity.Common;
using Healthtrackr.Activity.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Healthtrackr.Activity.Functions
{
    public class ManuallyRetrieveActivitySummaryByDate
    {
        private readonly IFitbitService _fitbitService;
        private readonly IActivityService _activityService;
        private readonly ILogger<ManuallyRetrieveActivitySummaryByDate> _logger;

        public ManuallyRetrieveActivitySummaryByDate(IFitbitService fitbitService,
            IActivityService activityService,
            ILogger<ManuallyRetrieveActivitySummaryByDate> logger)
        {
            _fitbitService = fitbitService;
            _activityService = activityService;
            _logger = logger;
        }

        [Function(nameof(ManuallyRetrieveActivitySummaryByDate))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{activityDate}")] HttpRequestData req, string activityDate)
        {
            IActionResult result;

            try
            {
                _logger.LogInformation($"{nameof(ManuallyRetrieveActivitySummaryByDate)} executed at: {DateTime.Now}");
                if (IsDateValid(activityDate) is false)
                {
                    result = new BadRequestResult();
                    return result;
                }

                _logger.LogInformation($"Attempting to retrieve Daily Activity Summary for {activityDate}");
                var activityResponse = await _fitbitService.GetActivityResponse(activityDate);

                _logger.LogInformation($"Mapping response to Activity object and Sending to queue.");
                await _activityService.MapActivityEnvelopeAndSaveToDatabase(activityDate, activityResponse);
                _logger.LogInformation($"Activity Summary sent to queue.");

                result = new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(ManuallyRetrieveActivitySummaryByDate)}: {ex.Message}");
                result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return result;
        }

        private bool IsDateValid(string date)
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
