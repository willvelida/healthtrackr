using FluentAssertions;
using System.Net;

namespace Healthtrackr.Api.Activity.IntegrationTests
{
    public class ActivityControllerShould : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _applicationFactory;

        public ActivityControllerShould(CustomWebApplicationFactory<Program> applicationFactory)
        {
            _applicationFactory = applicationFactory;
        }

        [Fact]
        public async Task ReturnBadRequestWhenCallingGetActivityEnvelopeByDateWithInvalidDateFormat()
        {
            // ARRANGE
            var testDate = DateTime.UtcNow.AddDays(-5).ToString("dd-MM-yyyy");
            var client = _applicationFactory.CreateClient();

            // ACT
            var response = await client.GetAsync(Environment.GetEnvironmentVariable("BlUE_SLOT_URL") + $"/api/activity?date=${testDate}");

            // ASSERT
            response.Should().HaveStatusCode(HttpStatusCode.BadRequest);
            response.Should().HaveError($"Date in format: {testDate} is invalid. Please provide a date in the format yyyy-MM-dd");
        }

        [Fact]
        public async Task ReturnNotFoundWhenCallingGetActivityEnvelopeByDateWhenRecordIsNull()
        {
            // ARRANGE
            var testDate = DateTime.UtcNow.AddYears(1).ToString("yyyy-MM-dd");
            var client = _applicationFactory.CreateClient();

            // ACT
            var response = await client.GetAsync(Environment.GetEnvironmentVariable("BlUE_SLOT_URL") + $"/api/activity?date={testDate}");

            // ASSERT
            response.Should().HaveStatusCode(HttpStatusCode.NotFound);
            response.Should().HaveError($"No activity envelope found for {testDate}");
        }

        [Fact]
        public async Task ReturnOkWhenCallingGetActivityEnvelopeByDateWithValidDate()
        {
            // ARRANGE
            var testDate = DateTime.UtcNow.AddDays(-5).ToString("yyyy-MM-dd");
            var client = _applicationFactory.CreateClient();

            // ACT
            var response = await client.GetAsync(Environment.GetEnvironmentVariable("BlUE_SLOT_URL") + $"/api/activity?date={testDate}");

            // ASSERT
            response.Should().HaveStatusCode(HttpStatusCode.OK);
        }

        [Fact]
        public async Task ReturnBadRequestWhenCallingGetActivitySummaryRecordByDateWithInvalidDate()
        {
            // ARRANGE
            var testDate = DateTime.UtcNow.AddDays(-5).ToString("dd-MM-yyyy");
            var client = _applicationFactory.CreateClient();

            // ACT
            var response = await client.GetAsync(Environment.GetEnvironmentVariable("BlUE_SLOT_URL") + $"/api/activitysummary?date={testDate}");

            // ASSERT
            response.Should().HaveStatusCode(HttpStatusCode.BadRequest);
            response.Should().HaveError($"Date in format: {testDate} is invalid. Please provide a date in the format yyyy-MM-dd");
        }

        [Fact]
        public async Task ReturnNotFoundWhenCallingGetActivitySummaryRecordByDateWhenRecordIsNull()
        {
            // ARRANGE
            var testDate = DateTime.UtcNow.AddYears(1).ToString("yyyy-MM-dd");
            var client = _applicationFactory.CreateClient();

            // ACT
            var response = await client.GetAsync(Environment.GetEnvironmentVariable("BlUE_SLOT_URL") + $"/api/activitysummary?date={testDate}");

            // ASSERT
            response.Should().HaveStatusCode(HttpStatusCode.NotFound);
            response.Should().HaveError($"No activity summary found for {testDate}");
        }

        [Fact]
        public async Task ReturnOkWhenCallingGetActivitySummaryRecordByDateWithValidDate()
        {
            // ARRANGE
            var testDate = DateTime.UtcNow.AddDays(-5).ToString("yyyy-MM-dd");
            var client = _applicationFactory.CreateClient();

            // ACT
            var response = await client.GetAsync(Environment.GetEnvironmentVariable("BlUE_SLOT_URL") + $"/api/activitysummary?date={testDate}");

            // ASSERT
            response.Should().HaveStatusCode(HttpStatusCode.OK);
        }

        [Fact]
        public async Task ReturnBadRequestWhenCallingGetActivitiesByDateWithInvalidDate()
        {
            // ARRANGE
            var testDate = DateTime.UtcNow.AddDays(-5).ToString("dd-MM-yyyy");
            var client = _applicationFactory.CreateClient();

            // ACT
            var response = await client.GetAsync(Environment.GetEnvironmentVariable("BlUE_SLOT_URL") + $"/api/activities?date={testDate}");

            // ASSERT
            response.Should().HaveStatusCode(HttpStatusCode.BadRequest);
            response.Should().HaveError($"Date in format: {testDate} is invalid. Please provide a date in the format yyyy-MM-dd");
        }

        [Fact]
        public async Task ReturnOkWhenCallingGetActivitiesByDateByDateWithValidDate()
        {
            // ARRANGE
            var testDate = DateTime.UtcNow.AddDays(-5).ToString("yyyy-MM-dd");
            var client = _applicationFactory.CreateClient();

            // ACT
            var response = await client.GetAsync(Environment.GetEnvironmentVariable("BlUE_SLOT_URL") + $"/api/activities?date={testDate}");

            // ASSERT
            response.Should().HaveStatusCode(HttpStatusCode.OK);
        }
    }
}