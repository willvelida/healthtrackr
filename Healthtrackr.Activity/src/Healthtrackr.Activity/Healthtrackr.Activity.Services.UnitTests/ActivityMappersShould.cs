using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging;
using Moq;
using res = Healthtrackr.Activity.Common.FitbitResponses;

namespace Healthtrackr.Activity.Services.UnitTests
{
    public class ActivityMappersShould
    {
        private Mock<ILogger<ActivityMappers>> _loggerMock;

        private ActivityMappers _activityMappersSut;

        public ActivityMappersShould()
        {
            _loggerMock = new Mock<ILogger<ActivityMappers>>();

            _activityMappersSut = new ActivityMappers(_loggerMock.Object);
        }

        [Fact]
        public void MapActivityToActivityRecordSuccessfully()
        {
            // ARRANGE
            var fixture = new Fixture();
            var testActivity = fixture.Create<res.Activity>();

            // ACT
            var expectedActivityRecord = _activityMappersSut.MapActivityToActivityRecord(testActivity);

            // ASSERT
            using (new AssertionScope())
            {
                expectedActivityRecord.ActivityName.Should().Be(testActivity.name);
                expectedActivityRecord.Calories.Should().Be(testActivity.calories);
                expectedActivityRecord.Duration.Should().Be(testActivity.duration);
                expectedActivityRecord.Date.Should().Be(testActivity.startDate);
                expectedActivityRecord.Time.Should().Be(testActivity.startTime);
                expectedActivityRecord.Steps.Should().Be(testActivity.steps);
            }
        }

        [Fact]
        public void MapSummaryToActivitySummaryRecordSuccessfully()
        {
            // ARRANGE
            var fixture = new Fixture();
            var testSummary = fixture.Create<res.Summary>();

            // ACT
            var expectedSummaryRecord = _activityMappersSut.MapSummaryToActivitySummaryRecord(testSummary);

            // ASSERT
            using (new AssertionScope())
            {
                expectedSummaryRecord.CaloriesBurned.Should().Be(testSummary.caloriesOut);
                expectedSummaryRecord.Steps.Should().Be(testSummary.steps);
                expectedSummaryRecord.Floors.Should().Be(testSummary.floors);
                expectedSummaryRecord.Distance.Should().Be(testSummary.distances.Where(d => d.activity == "total").Select(d => d.distance).FirstOrDefault());
                expectedSummaryRecord.MinutesSedentary.Should().Be(testSummary.sedentaryMinutes);
                expectedSummaryRecord.MinutesLightlyActive.Should().Be(testSummary.lightlyActiveMinutes);
                expectedSummaryRecord.MinutesFairlyActive.Should().Be(testSummary.fairlyActiveMinutes);
                expectedSummaryRecord.MinutesVeryActive.Should().Be(testSummary.veryActiveMinutes);
                expectedSummaryRecord.ActivityCalories.Should().Be(testSummary.activityCalories);
            }
        }

        [Fact]
        public void ThrowExceptionWhenActivityIsNull()
        {
            Action mapperAction = () => _activityMappersSut.MapActivityToActivityRecord(null);

            mapperAction.Should().Throw<Exception>();
            _loggerMock.VerifyLog(logger => logger.LogError($"Exception thrown in MapActivityToActivityRecord: Object reference not set to an instance of an object."));
        }

        [Fact]
        public void ThrowExceptionWhenSummaryIsNull()
        {
            Action mapperAction = () => _activityMappersSut.MapSummaryToActivitySummaryRecord(null);

            mapperAction.Should().Throw<Exception>();
            _loggerMock.VerifyLog(logger => logger.LogError($"Exception thrown in MapSummaryToActivitySummaryRecord: Object reference not set to an instance of an object."));
        }
    }
}
