using Healthtrackr.Api.Activity.Common.Filters;
using Healthtrackr.Api.Activity.Common.Models;
using Healthtrackr.Api.Activity.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Healthtrackr.Api.Activity.Controllers
{
    [ApiController]
    [Route("api/activity")]
    public class ActivityController : ControllerBase
    {
        private readonly IActivityService _activityService;
        private readonly ILogger<ActivityController> _logger;

        public ActivityController(IActivityService activityService, ILogger<ActivityController> logger)
        {
            _activityService = activityService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ActivityRecord>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllActivityRecords([FromQuery] PaginationFilter paginationFilter)
        {
            try
            {
                var activityRecords = await _activityService.GetAllActivityRecords(paginationFilter, Request.Path.Value);
                return Ok(activityRecords);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetAllActivityRecords)}: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [Route("{activityDate}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ActivityRecord))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetActivitiesByDate(string activityDate)
        {
            try
            {
                var isDateValid = _activityService.IsDateValid(activityDate);
                if (isDateValid is false)
                {
                    return BadRequest($"Date in format: {activityDate} is invalid. Please provide a date in the format yyyy-MM-dd");
                }

                var activityRecords = await _activityService.GetActivityRecords(activityDate);

                return Ok(activityRecords);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetActivitiesByDate)}: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
