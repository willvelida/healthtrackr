using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using Healthtrackr.Api.Activity.Common.Models;
using Healthtrackr.Api.Activity.Repository.Interfaces;
using Healthtrackr.Api.Activity.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace Healthtrackr.Api.Activity.Services.UnitTests
{
    public class ActivityServiceShould
    {
        private Mock<ICosmosDbRepository> _cosmosRepoMock;
        private Mock<IActivityRepository> _activityRepoMock;
        private Mock<IUriService> _uriServiceMock;
        private Mock<ILogger<ActivityService>> _loggerMock;

        private ActivityService _activityServiceSut;

        public ActivityServiceShould()
        {
            _cosmosRepoMock = new Mock<ICosmosDbRepository>();
            _activityRepoMock = new Mock<IActivityRepository>();
            _uriServiceMock = new Mock<IUriService>();
            _loggerMock = new Mock<ILogger<ActivityService>>();

            _activityServiceSut = new ActivityService(_cosmosRepoMock.Object, _activityRepoMock.Object, _uriServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task ReturnOneEnvelopeWhenCallingGetActivityEnvelope()
        {
            // ARRANGE
            var fixture = new Fixture();
            var envelopeList = fixture.Build<ActivityEnvelope>().CreateMany(1).ToList();
            var testDate = DateTime.Now.ToString("yyyy-MM-dd");
            envelopeList[0].Date = testDate;

            _cosmosRepoMock.Setup(
                x => x.GetActivityEnvelopeByDate(It.IsAny<string>()))
                .ReturnsAsync(envelopeList);

            // ACT
            var expectedEnvelopes = await _activityServiceSut.GetActivityEnvelope(testDate);

            // ASSERT
            using (new AssertionScope())
            {
                expectedEnvelopes.Should().NotBeNull();
                expectedEnvelopes.Date.Should().Be(testDate);
            }
        }

        [Fact]
        public async Task ThrowExceptionWhenCosmosDbRepositoryFails()
        {
            // ARRANGE
            var testDate = DateTime.Now.ToString("yyyy-MM-dd");

            _cosmosRepoMock.Setup(
                x => x.GetActivityEnvelopeByDate(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Mock Exception"));

            // ACT
            Func<Task> activityServiceAction = async () => await _activityServiceSut.GetActivityEnvelope(testDate);

            // ASSERT
            await activityServiceAction.Should().ThrowAsync<Exception>().WithMessage("Mock Exception");
            _loggerMock.VerifyLog(logger => logger.LogError("Exception thrown in GetActivityEnvelope: Mock Exception"));
        }

        [Fact]
        public async Task ReturnActivityRecordsWhenCallingGetActivityRecordsByDate()
        {
            // ARRANGE
            var testDate = DateTime.Now.ToString("yyyy-MM-dd");
            var fixture = new Fixture();
            var recordList = fixture.Build<ActivityRecord>().CreateMany(1).ToList();
            recordList[0].Date = DateTime.Parse(testDate);

            _activityRepoMock
                .Setup(x => x.GetActivityRecordsByDate(It.IsAny<string>()))
                .ReturnsAsync(recordList);

            // ACT
            var activityRecords = await _activityServiceSut.GetActivityRecords(testDate);

            // ASSERT
            using (new AssertionScope())
            {
                activityRecords.Should().NotBeNull();
                activityRecords[0].Date.Should().Be(DateTime.Parse(testDate));
            }
        }

        [Fact]
        public async Task ThrowExceptionWhenGetActivityRecordsFail()
        {
            // ARRANGE
            var testDate = DateTime.Now.ToString("yyyy-MM-dd");

            _activityRepoMock
                .Setup(x => x.GetActivityRecordsByDate(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Mock Exception"));

            // ACT
            Func<Task> activityServiceAction = async () => await _activityServiceSut.GetActivityRecords(testDate);

            // ASSERT
            await activityServiceAction.Should().ThrowAsync<Exception>().WithMessage("Mock Exception");
            _loggerMock.VerifyLog(logger => logger.LogError("Exception thrown in GetActivityRecords: Mock Exception"));
        }

        [Fact]
        public async Task ReturnActivitySummaryWhenCallingGetActivitySummary()
        {
            // ARRANGE
            var testDate = DateTime.Now.ToString("yyyy-MM-dd");
            var fixture = new Fixture();
            var activitySummary = fixture.Create<ActivitySummaryRecord>();
            activitySummary.Date = DateTime.Parse(testDate);

            _activityRepoMock
                .Setup(x => x.GetActivitySummaryRecordByDate(It.IsAny<string>()))
                .ReturnsAsync(activitySummary);

            // ACT
            var activitySummaryRecord = await _activityServiceSut.GetActivitySummary(testDate);

            // ASSERT
            using (new AssertionScope())
            {
                activitySummaryRecord.Should().NotBeNull();
                activitySummaryRecord.Date.Should().Be(DateTime.Parse(testDate));
            }
        }

        [Fact]
        public async Task ThrowExceptionWhenGetActivitySummaryFail()
        {
            // ARRANGE
            var testDate = DateTime.Now.ToString("yyyy-MM-dd");

            _activityRepoMock
                .Setup(x => x.GetActivitySummaryRecordByDate(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Mock Exception"));

            // ACT
            Func<Task> activityServiceAction = async () => await _activityServiceSut.GetActivitySummary(testDate);

            // ASSERT
            await activityServiceAction.Should().ThrowAsync<Exception>().WithMessage("Mock Exception");
            _loggerMock.VerifyLog(logger => logger.LogError("Exception thrown in GetActivitySummary: Mock Exception"));
        }

        [Theory]
        [InlineData("25/06/2023")]
        [InlineData("06/25/2023")]
        [InlineData("06-25-2023")]
        [InlineData("25-06-2023")]
        public void ReturnFalseWhenPassingIncorrectDateFormatToIsDateValid(string testDate)
        {
            // ACT
            var expectedFalseResponse = _activityServiceSut.IsDateValid(testDate);

            // ASSERT
            expectedFalseResponse.Should().BeFalse();
        }

        [Fact]
        public void ReturnTrueWhenPassingCorrectDateFormatToIsDateValid()
        {
            // ARRANGE
            var testDate = DateTime.Now.ToString("yyyy-MM-dd");

            // ACT
            var expectedFalseResponse = _activityServiceSut.IsDateValid(testDate);

            // ASSERT
            expectedFalseResponse.Should().BeTrue();
        }
    }
}