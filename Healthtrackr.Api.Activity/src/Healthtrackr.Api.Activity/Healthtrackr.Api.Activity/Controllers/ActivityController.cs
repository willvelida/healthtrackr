using Healthtrackr.Api.Activity.Common.Models;
using Healthtrackr.Api.Activity.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Healthtrackr.Api.Activity.Controllers
{
    [Route("api/activity")]
    [ApiController]
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ActivityEnvelope))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetActivityEnvelopeByDate(string date)
        {
            try
            {
                var isDateValid = _activityService.IsDateValid(date);
                if (isDateValid is false)
                {
                    return BadRequest($"Date in format: {date} is invalid. Please provide a date in the formate yyyy-MM-dd");
                }

                var activityEnvelope = await _activityService.GetActivityEnvelope(date);
                if (activityEnvelope is null)
                {
                    return NotFound($"No activity envelope found for {date}");
                }

                return Ok(activityEnvelope);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetActivityEnvelopeByDate)}: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
