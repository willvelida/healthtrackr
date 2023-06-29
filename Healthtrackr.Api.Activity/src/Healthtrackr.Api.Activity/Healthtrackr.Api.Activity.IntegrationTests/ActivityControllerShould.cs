using Azure.Identity;
using FluentAssertions;
using Healthtrackr.Api.Activity.Repository.Interfaces;
using Healthtrackr.Api.Activity.Repository;
using Healthtrackr.Api.Activity.Services.Interfaces;
using Healthtrackr.Api.Activity.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Healthtrackr.Api.Activity.Common;

namespace Healthtrackr.Api.Activity.IntegrationTests
{
    public class ActivityControllerShould : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _applicationFactory;
        private readonly HttpClient _httpClient;

        public ActivityControllerShould()
        {
            _applicationFactory = new WebApplicationFactory<Program>();
            _httpClient = _applicationFactory.CreateClient();
        }

        [Fact]
        public async Task ReturnBadRequestWhenCallingGetActivityEnvelopeByDateWithInvalidDateFormat()
        {
            // ARRANGE
            var testDate = DateTime.UtcNow.AddDays(-5).ToString("dd-MM-yyyy");

            // ACT
            var response = await _httpClient.GetAsync(Environment.GetEnvironmentVariable("BlUE_SLOT_URL") + $"/api/activity?date=${testDate}");

            // ASSERT
            response.Should().HaveStatusCode(HttpStatusCode.BadRequest);
            response.Should().HaveError($"Date in format: {testDate} is invalid. Please provide a date in the format yyyy-MM-dd");
        }

        [Fact]
        public async Task ReturnNotFoundWhenCallingGetActivityEnvelopeByDateWhenRecordIsNull()
        {
            // ARRANGE
            var testDate = DateTime.UtcNow.AddYears(1).ToString("yyyy-MM-dd");

            // ACT
            var response = await _httpClient.GetAsync(Environment.GetEnvironmentVariable("BlUE_SLOT_URL") + $"/api/activity?date=${testDate}");

            // ASSERT
            response.Should().HaveStatusCode(HttpStatusCode.NotFound);
            response.Should().HaveError($"No activity envelope found for {testDate}");
        }

        [Fact]
        public async Task ReturnOkWhenCallingGetActivityEnvelopeByDateWithValidDate()
        {
            // ARRANGE
            var testDate = DateTime.UtcNow.AddDays(-5).ToString("yyyy-MM-dd");

            // ACT
            var response = await _httpClient.GetAsync(Environment.GetEnvironmentVariable("BlUE_SLOT_URL") + $"/api/activity?date=${testDate}");

            // ASSERT
            response.Should().HaveStatusCode(HttpStatusCode.OK);
        }

        [Fact]
        public async Task ReturnBadRequestWhenCallingGetActivitySummaryRecordByDateWithInvalidDate()
        {
            // ARRANGE
            var testDate = DateTime.UtcNow.AddDays(-5).ToString("dd-MM-yyyy");

            // ACT
            var response = await _httpClient.GetAsync(Environment.GetEnvironmentVariable("BlUE_SLOT_URL") + $"/api/activitysummary?date=${testDate}");

            // ASSERT
            response.Should().HaveStatusCode(HttpStatusCode.BadRequest);
            response.Should().HaveError($"Date in format: {testDate} is invalid. Please provide a date in the format yyyy-MM-dd");
        }

        [Fact]
        public async Task ReturnNotFoundWhenCallingGetActivitySummaryRecordByDateWhenRecordIsNull()
        {
            // ARRANGE
            var testDate = DateTime.UtcNow.AddYears(1).ToString("yyyy-MM-dd");

            // ACT
            var response = await _httpClient.GetAsync(Environment.GetEnvironmentVariable("BlUE_SLOT_URL") + $"/api/activitysummary?date=${testDate}");

            // ASSERT
            response.Should().HaveStatusCode(HttpStatusCode.NotFound);
            response.Should().HaveError($"No activity summary found for {testDate}");
        }

        [Fact]
        public async Task ReturnOkWhenCallingGetActivitySummaryRecordByDateWithValidDate()
        {
            // ARRANGE
            var testDate = DateTime.UtcNow.AddDays(-5).ToString("yyyy-MM-dd");

            // ACT
            var response = await _httpClient.GetAsync(Environment.GetEnvironmentVariable("BlUE_SLOT_URL") + $"/api/activitysummary?date=${testDate}");

            // ASSERT
            response.Should().HaveStatusCode(HttpStatusCode.OK);
        }

        [Fact]
        public async Task ReturnBadRequestWhenCallingGetActivitiesByDateWithInvalidDate()
        {
            // ARRANGE
            var testDate = DateTime.UtcNow.AddDays(-5).ToString("dd-MM-yyyy");

            // ACT
            var response = await _httpClient.GetAsync(Environment.GetEnvironmentVariable("BlUE_SLOT_URL") + $"/api/activities?date=${testDate}");

            // ASSERT
            response.Should().HaveStatusCode(HttpStatusCode.BadRequest);
            response.Should().HaveError($"Date in format: {testDate} is invalid. Please provide a date in the format yyyy-MM-dd");
        }

        [Fact]
        public async Task ReturnOkWhenCallingGetActivitiesByDateByDateWithValidDate()
        {
            // ARRANGE
            var testDate = DateTime.UtcNow.AddDays(-5).ToString("yyyy-MM-dd");

            // ACT
            var response = await _httpClient.GetAsync(Environment.GetEnvironmentVariable("BlUE_SLOT_URL") + $"/api/activities?date=${testDate}");

            // ASSERT
            response.Should().HaveStatusCode(HttpStatusCode.OK);
        }
    }
}