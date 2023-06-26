using AutoFixture;
using Azure.Messaging.ServiceBus;
using FluentAssertions;
using Healthtrackr.Activity.Common.Models;
using Healthtrackr.Activity.Repository.Interfaces;
using Healthtrackr.Activity.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using res = Healthtrackr.Activity.Common.FitbitResponses;

namespace Healthtrackr.Activity.Services.UnitTests
{
    public class ActivityServiceShould
    {
        private Mock<ServiceBusClient> _serviceBusMock;
        private Mock<ServiceBusSender> _serviceBusSenderMock;
        private Mock<IActivityMappers> _mapperMock;
        private Mock<IActivityRepository> _activityRepoMock;
        private Mock<ICosmosDbRepository> _cosmosRepoMock;
        private Mock<ILogger<ActivityService>> _loggerMock;

        private ActivityService _activityServiceSut;

        public ActivityServiceShould()
        {
            _serviceBusMock = new Mock<ServiceBusClient>();
            _serviceBusSenderMock = new Mock<ServiceBusSender>();
            _cosmosRepoMock = new Mock<ICosmosDbRepository>();
            _mapperMock = new Mock<IActivityMappers>();
            _activityRepoMock = new Mock<IActivityRepository>();
            _loggerMock = new Mock<ILogger<ActivityService>>();

            _activityServiceSut = new ActivityService(_serviceBusMock.Object, _cosmosRepoMock.Object, _loggerMock.Object, _mapperMock.Object, _activityRepoMock.Object);
        }

        [Fact]
        public async Task MapAndSaveActivityEnvelopeSuccessfully()
        {
            // ARRANGE
            var fixture = new Fixture();
            var activityResponse = fixture.Create<res.ActivityResponse>();
            var date = "2022-12-31";

            _cosmosRepoMock
                .Setup(x => x.CreateActivityDocument(It.IsAny<ActivityEnvelope>()))
                .Returns(Task.CompletedTask);

            // ACT
            Func<Task> activityServiceAction = async () => await _activityServiceSut.MapActivityEnvelopeAndSaveToDatabase(date, activityResponse);

            // ASSERT
            await activityServiceAction.Should().NotThrowAsync<Exception>();
            _cosmosRepoMock.Verify(x => x.CreateActivityDocument(It.IsAny<ActivityEnvelope>()), Times.Once);
        }

        [Fact]
        public async Task ThrowExceptionWhenMapAndSaveActivityEnvelopeFails()
        {
            // ARRANGE
            var fixture = new Fixture();
            var activityResponse = fixture.Create<res.ActivityResponse>();
            var date = "2022-12-31";

            _cosmosRepoMock
                .Setup(x => x.CreateActivityDocument(It.IsAny<ActivityEnvelope>()))
                .ThrowsAsync(new Exception("Mock Failure"));

            // ACT
            Func<Task> activityServiceAction = async () => await _activityServiceSut.MapActivityEnvelopeAndSaveToDatabase(date, activityResponse);

            // ASSERT
            await activityServiceAction.Should().ThrowAsync<Exception>();
            _loggerMock.VerifyLog(logger => logger.LogError($"Exception thrown in MapActivityEnvelopeAndSaveToDatabase: Mock Failure"));
        }

        [Fact]
        public async Task MapAndSaveActivityRecordsSuccessfully()
        {
            // ARRANGE
            var fixture = new Fixture();
            var activityEnvelope = fixture.Create<ActivityEnvelope>();
            var testActivityRecord = fixture.Create<ActivityRecord>();
            activityEnvelope.Date = "2023-01-01";

            _mapperMock
                .Setup(x => x.MapActivityToActivityRecord(It.IsAny<res.Activity>()))
                .Returns(testActivityRecord);

            _activityRepoMock
                .Setup(x => x.AddActivityRecord(It.IsAny<ActivityRecord>()))
                .Returns(Task.CompletedTask);

            // ACT
            Func<Task> activityServiceAction = async () => await _activityServiceSut.MapAndSaveActivityRecords(activityEnvelope);

            // ASSERT
            await activityServiceAction.Should().NotThrowAsync<Exception>();
        }

        [Fact]
        public async Task NotThrowExceptionWhenActivitiesAreNull()
        {
            // ARRANGE
            var fixture = new Fixture();
            var activityEnvelope = fixture.Create<ActivityEnvelope>();
            activityEnvelope.Date = "2023-01-01";
            activityEnvelope.Activity.activities = null;

            // ACT
            Func<Task> activityServiceAction = async () => await _activityServiceSut.MapAndSaveActivityRecords(activityEnvelope);

            // ASSERT
            await activityServiceAction.Should().NotThrowAsync<NullReferenceException>();
            _loggerMock.VerifyLog(logger => logger.LogInformation($"No Activities to map. Exiting."));
        }

        [Fact]
        public async Task ThrowExceptionWhenAddActivityRecordFails()
        {
            // ARRANGE
            var fixture = new Fixture();
            var activityEnvelope = fixture.Create<ActivityEnvelope>();
            var testActivityRecord = fixture.Create<ActivityRecord>();
            activityEnvelope.Date = "2023-01-01";

            _mapperMock
                .Setup(x => x.MapActivityToActivityRecord(It.IsAny<res.Activity>()))
                .Returns(testActivityRecord);

            _activityRepoMock
                .Setup(x => x.AddActivityRecord(It.IsAny<ActivityRecord>()))
                .ThrowsAsync(new Exception("Mock Failure"));

            // ACT
            Func<Task> activityServiceAction = async () => await _activityServiceSut.MapAndSaveActivityRecords(activityEnvelope);

            // ASSERT
            await activityServiceAction.Should().ThrowAsync<Exception>();
            _loggerMock.VerifyLog(logger => logger.LogError($"Exception thrown in MapAndSaveActivityRecords: Mock Failure"));
        }

        [Fact]
        public async Task MapAndSaveActivitySummaryRecordSuccessfully()
        {
            // ARRANGE
            var fixture = new Fixture();
            var activityEnvelope = fixture.Create<ActivityEnvelope>();
            var testActivitySummary = new ActivitySummaryRecord();
            activityEnvelope.Date = "2023-01-01";

            _mapperMock
                .Setup(x => x.MapSummaryToActivitySummaryRecord(It.IsAny<res.Summary>()))
                .Returns(testActivitySummary);

            _activityRepoMock
                .Setup(x => x.AddActivitySummaryRecord(It.IsAny<ActivitySummaryRecord>()))
                .Returns(Task.CompletedTask);

            // ACT
            Func<Task> activityServiceAction = async () => await _activityServiceSut.MapAndSaveActivitySummaryRecord(activityEnvelope);

            // ASSERT
            await activityServiceAction.Should().NotThrowAsync<Exception>();
        }

        [Fact]
        public async Task NotThrowExceptionWhenSummaryAreNull()
        {
            // ARRANGE
            var fixture = new Fixture();
            var activityEnvelope = fixture.Create<ActivityEnvelope>();
            activityEnvelope.Date = "2023-01-01";
            activityEnvelope.Activity.summary = null;

            // ACT
            Func<Task> activityServiceAction = async () => await _activityServiceSut.MapAndSaveActivitySummaryRecord(activityEnvelope);

            // ASSERT
            await activityServiceAction.Should().NotThrowAsync<NullReferenceException>();
            _loggerMock.VerifyLog(logger => logger.LogInformation($"No Activity Summaries to map. Exiting."));
        }

        [Fact]
        public async Task ThrowExceptionWhenAddActivitySummaryRecordFails()
        {
            // ARRANGE
            var fixture = new Fixture();
            var activityEnvelope = fixture.Create<ActivityEnvelope>();
            var testActivitySummary = new ActivitySummaryRecord();
            activityEnvelope.Date = "2023-01-01";

            _mapperMock
                .Setup(x => x.MapSummaryToActivitySummaryRecord(It.IsAny<res.Summary>()))
                .Returns(testActivitySummary);

            _activityRepoMock
                .Setup(x => x.AddActivitySummaryRecord(It.IsAny<ActivitySummaryRecord>()))
                .ThrowsAsync(new Exception("Mock Failure"));

            // ACT
            Func<Task> activityServiceAction = async () => await _activityServiceSut.MapAndSaveActivitySummaryRecord(activityEnvelope);

            // ASSERT
            await activityServiceAction.Should().ThrowAsync<Exception>();
            _loggerMock.VerifyLog(logger => logger.LogError($"Exception thrown in MapAndSaveActivitySummaryRecord: Mock Failure"));
        }

        [Fact]
        public async Task SendActivityRecordToQueue()
        {
            // ARRANGE
            var fixture = new Fixture();
            var activityResponse = fixture.Create<res.ActivityResponse>();
            var queueName = "activityqueue";

            _serviceBusMock
                .Setup(x => x.CreateSender(It.IsAny<string>()))
                .Returns(_serviceBusSenderMock.Object);

            _serviceBusSenderMock
                .Setup(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // ACT
            Func<Task> activityServiceAction = async () => await _activityServiceSut.SendRecordToQueue(activityResponse, queueName);

            // ASSERT
            await activityServiceAction.Should().NotThrowAsync<Exception>();
            _serviceBusSenderMock.Verify(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ThrowExceptionWhenSendingRecordToQueueFails()
        {
            // ARRANGE
            var fixture = new Fixture();
            var activityResponse = fixture.Create<res.ActivityResponse>();
            var queueName = "activityqueue";

            _serviceBusMock
                .Setup(x => x.CreateSender(It.IsAny<string>()))
                .Returns(_serviceBusSenderMock.Object);

            _serviceBusSenderMock
                .Setup(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Mock Failure"));

            // ACT
            Func<Task> activityServiceAction = async () => await _activityServiceSut.SendRecordToQueue(activityResponse, queueName);

            // ASSERT
            await activityServiceAction.Should().ThrowAsync<Exception>();
            _loggerMock.VerifyLog(logger => logger.LogError($"Exception thrown in SendRecordToQueue: Mock Failure"));
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
            var testDate = "2023-06-25";

            // ACT
            var expectedFalseResponse = _activityServiceSut.IsDateValid(testDate);

            // ASSERT
            expectedFalseResponse.Should().BeTrue();
        }
    }
}