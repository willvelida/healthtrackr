using Healthtrackr.Api.Activity.Common.Models;
using Healthtrackr.Api.Activity.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Healthtrackr.Api.Activity.Controllers
{
    [Route("api/activitydetails")]
    [ApiController]
    public class ActivityDetailsController : ControllerBase
    {
        private readonly IActivityService _activityService;
        private readonly ILogger<ActivityDetailsController> _logger;

        public ActivityDetailsController(IActivityService activityService, ILogger<ActivityDetailsController> logger)
        {
            _activityService = activityService;
            _logger = logger;
        }

        [HttpGet("{activityDate}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ActivityEnvelope))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetActivityDetailsByDate(string activityDate)
        {
            try
            {
                var isDateValid = _activityService.IsDateValid(activityDate);
                if (isDateValid is false)
                {
                    return BadRequest($"Date in format: {activityDate} is invalid. Please provide a date in the format yyyy-MM-dd");
                }

                var activityEnvelope = await _activityService.GetActivityEnvelope(activityDate);
                if (activityEnvelope is null)
                {
                    return NotFound($"No activity envelope found for {activityDate}");
                }

                return Ok(activityEnvelope);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetActivityDetailsByDate)}: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
