using Healthtrackr.Api.Activity.Common.Filters;
using Healthtrackr.Api.Activity.Common.Models;
using Healthtrackr.Api.Activity.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Healthtrackr.Api.Activity.Controllers
{
    [Route("api/activitysummary")]
    [ApiController]
    public class ActivitySummaryController : ControllerBase
    {
        private readonly IActivityService _activityService;
        private readonly ILogger<ActivitySummaryController> _logger;

        public ActivitySummaryController(IActivityService activityService, ILogger<ActivitySummaryController> logger)
        {
            _activityService = activityService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ActivitySummaryRecord>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllActivitySummaryRecords([FromQuery] PaginationFilter paginationFilter)
        {
            try
            {
                var activitySummaryRecords = await _activityService.GetAllActivitySummaryRecords(paginationFilter, Request.Path.Value);
                return Ok(activitySummaryRecords);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetAllActivitySummaryRecords)}: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [Route("{activityDate}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ActivitySummaryRecord))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetActivitySummaryRecordByDate(string activityDate)
        {
            try
            {
                var isDateValid = _activityService.IsDateValid(activityDate);
                if (isDateValid is false)
                {
                    return BadRequest($"Date in format: {activityDate} is invalid. Please provide a date in the format yyyy-MM-dd");
                }

                var activitySummaryRecord = await _activityService.GetActivitySummary(activityDate);
                if (activitySummaryRecord is null)
                {
                    return NotFound($"No activity summary found for {activityDate}");
                }

                return Ok(activitySummaryRecord);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetActivitySummaryRecordByDate)}: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
