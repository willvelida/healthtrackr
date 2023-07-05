using Healthtrackr.Api.Activity.Common.Filters;
using Healthtrackr.Api.Activity.Common.Models;
using Healthtrackr.Api.Activity.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace Healthtrackr.Api.Activity.Controllers
{
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

        [Route("api/activityrecords")]
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

        [Route("api/activitysummaryrecords")]
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

        [Route("api/activity")]
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
                    return BadRequest($"Date in format: {date} is invalid. Please provide a date in the format yyyy-MM-dd");
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

        [Route("api/activitysummary")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ActivitySummaryRecord))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetActivitySummaryRecordByDate(string date)
        {
            try
            {
                var isDateValid = _activityService.IsDateValid(date);
                if (isDateValid is false)
                {
                    return BadRequest($"Date in format: {date} is invalid. Please provide a date in the format yyyy-MM-dd");
                }

                var activitySummaryRecord = await _activityService.GetActivitySummary(date);
                if (activitySummaryRecord is null)
                {
                    return NotFound($"No activity summary found for {date}");
                }

                return Ok(activitySummaryRecord);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetActivitySummaryRecordByDate)}: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [Route("api/activities")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ActivityRecord))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetActivitiesByDate(string date)
        {
            try
            {
                var isDateValid = _activityService.IsDateValid(date);
                if (isDateValid is false)
                {
                    return BadRequest($"Date in format: {date} is invalid. Please provide a date in the format yyyy-MM-dd");
                }

                var activityRecords = await _activityService.GetActivityRecords(date);

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
