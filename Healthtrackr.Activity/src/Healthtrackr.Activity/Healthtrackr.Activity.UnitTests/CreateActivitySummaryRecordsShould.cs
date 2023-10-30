using AutoFixture;
using FluentAssertions;
using Healthtrackr.Activity.Common.Models;
using Healthtrackr.Activity.Functions;
using Healthtrackr.Activity.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace Healthtrackr.Activity.UnitTests
{
    public class CreateActivitySummaryRecordsShould
    {
        private readonly Mock<IActivityService> _mockActivityService;
        private readonly Mock<ILogger<CreateActivitySummaryRecords>> _mockLogger;

        private CreateActivitySummaryRecords _sut;

        public CreateActivitySummaryRecordsShould()
        {
            _mockActivityService = new Mock<IActivityService>();
            _mockLogger = new Mock<ILogger<CreateActivitySummaryRecords>>();

            _sut = new CreateActivitySummaryRecords(_mockActivityService.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task SuccessfullyCreateActivitySummaryRecords()
        {
            // ARRANGE
            var fixture = new Fixture();
            var activityEnvelope = fixture.Create<ActivityEnvelope>();
            activityEnvelope.Date = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            var activityEnvelopes = new List<ActivityEnvelope>();
            activityEnvelopes.Add(activityEnvelope);

            _mockActivityService.Setup(x => x.MapAndSaveActivitySummaryRecord(It.IsAny<ActivityEnvelope>())).Returns(Task.CompletedTask);
            _mockActivityService.Setup(x => x.MapAndSaveActivityRecords(It.IsAny<ActivityEnvelope>())).Returns(Task.CompletedTask);

            // ACT
            Func<Task> functionAction = async () => await _sut.Run(activityEnvelopes);

            // ASSERT
            await functionAction.Should().NotThrowAsync<Exception>();
            _mockLogger.VerifyLog(logger => logger.LogInformation($"Records for {activityEnvelope.Date} saved"));
        }

        [Fact]
        public async Task ThrowExceptionWhenMapAndSaveActivitySummaryRecordFails()
        {
            // ARRANGE
            var fixture = new Fixture();
            var activityEnvelope = fixture.Create<ActivityEnvelope>();
            var activityEnvelopes = new List<ActivityEnvelope>();
            activityEnvelopes.Add(activityEnvelope);

            _mockActivityService.Setup(x => x.MapAndSaveActivitySummaryRecord(It.IsAny<ActivityEnvelope>())).ThrowsAsync(new Exception("Mock Failure"));

            // ACT
            Func<Task> functionAction = async () => await _sut.Run(activityEnvelopes);

            // ASSERT
            await functionAction.Should().ThrowAsync<Exception>();
            _mockLogger.VerifyLog(logger => logger.LogError($"Exception thrown in CreateActivitySummaryRecords: Mock Failure"));
        }

        [Fact]
        public async Task ThrowExceptionWhenMapAndSaveActivityRecordsFails()
        {
            // ARRANGE
            var fixture = new Fixture();
            var activityEnvelope = fixture.Create<ActivityEnvelope>();
            var activityEnvelopes = new List<ActivityEnvelope>();
            activityEnvelopes.Add(activityEnvelope);

            _mockActivityService.Setup(x => x.MapAndSaveActivitySummaryRecord(It.IsAny<ActivityEnvelope>())).Returns(Task.CompletedTask);
            _mockActivityService.Setup(x => x.MapAndSaveActivityRecords(It.IsAny<ActivityEnvelope>())).ThrowsAsync(new Exception("Mock Failure"));

            // ACT
            Func<Task> functionAction = async () => await _sut.Run(activityEnvelopes);

            // ASSERT
            await functionAction.Should().ThrowAsync<Exception>();
            _mockLogger.VerifyLog(logger => logger.LogError($"Exception thrown in CreateActivitySummaryRecords: Mock Failure"));
        }
    }
}
