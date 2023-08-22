﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace Healthtrackr.Api.Activity.IntegrationTests
{
    public class ActivityDetailsControllerShould : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _applicationFactory;

        public ActivityDetailsControllerShould(WebApplicationFactory<Program> applicationFactory)
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
    }
}
