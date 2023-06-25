using AutoFixture;
using AutoMapper;
using FluentAssertions;
using FluentAssertions.Execution;
using Healthtrackr.Activity.Common.FitbitResponses;
using Healthtrackr.Activity.Common.Models;
using Healthtrackr.Activity.Services.Mappers;

namespace Healthtrackr.Activity.Services.UnitTests
{
    public class MapSummaryToActivitySummaryRecordShould
    {
        private readonly IMapper _mapper;

        public MapSummaryToActivitySummaryRecordShould()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MapSummaryToActivitySummaryRecord());
            });

            _mapper = config.CreateMapper();
        }

        [Fact]
        public void MapResponseToRecord()
        {
            // ARRANGE
            var fixture = new Fixture();
            var summary = fixture.Create<Summary>();

            // ACT
            var expectedActivitySummaryRecord = _mapper.Map<ActivitySummaryRecord>(summary);

            // ASSERT
            using (new AssertionScope())
            {
                expectedActivitySummaryRecord.CaloriesOut.Should().Be(summary.caloriesOut);
                expectedActivitySummaryRecord.ActivityCalories.Should().Be(summary.activityCalories);
                expectedActivitySummaryRecord.Elevation.Should().Be(summary.elevation);
                expectedActivitySummaryRecord.FairlyActiveMinutes.Should().Be(summary.fairlyActiveMinutes);
                expectedActivitySummaryRecord.Floors.Should().Be(summary.floors);
                expectedActivitySummaryRecord.LightlyActiveMinutes.Should().Be(summary.lightlyActiveMinutes);
                expectedActivitySummaryRecord.SedentaryMinutes.Should().Be(summary.sedentaryMinutes);
                expectedActivitySummaryRecord.Steps.Should().Be(summary.steps);
                expectedActivitySummaryRecord.VeryActiveMinutes.Should().Be(summary.veryActiveMinutes);
            }
        }
    }
}
