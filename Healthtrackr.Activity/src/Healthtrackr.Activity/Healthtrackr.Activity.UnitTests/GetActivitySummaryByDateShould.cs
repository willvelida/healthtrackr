using AutoFixture;
using FluentAssertions;
using Healthtrackr.Activity.Common;
using Healthtrackr.Activity.Common.FitbitResponses;
using Healthtrackr.Activity.Functions;
using Healthtrackr.Activity.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Healthtrackr.Activity.UnitTests
{
    public class GetActivitySummaryByDateShould
    {
        private readonly Mock<IFitbitService> _mockFitbitService;
        private readonly Mock<IActivityService> _mockActivityService;
        private readonly Mock<ILogger<GetActivitySummaryByDate>> _mockLogger;
        private readonly Mock<IOptions<Settings>> _mockOptions;
        private Mock<TimerInfo> _mockTimerInfo;

        private GetActivitySummaryByDate _sut;

        public GetActivitySummaryByDateShould()
        {
            _mockFitbitService = new Mock<IFitbitService>();
            _mockActivityService = new Mock<IActivityService>();
            _mockLogger = new Mock<ILogger<GetActivitySummaryByDate>>();
            _mockTimerInfo = new Mock<TimerInfo>();
            _mockOptions = new Mock<IOptions<Settings>>();
            _mockOptions.Setup(x => x.Value).Returns(new Settings
            {
                ActivityQueueName = "testqueue"
            });

            _sut = new GetActivitySummaryByDate(
                _mockFitbitService.Object,
                _mockActivityService.Object,
                _mockLogger.Object,
                _mockOptions.Object);
        }

        [Fact]
        public async Task SuccessfullySendMessage()
        {
            // ARRANGE
            var fixture = new Fixture();
            var activityResponse = fixture.Create<ActivityResponse>();

            _mockFitbitService.Setup(x => x.GetActivityResponse(It.IsAny<string>())).ReturnsAsync(activityResponse);
            _mockActivityService.Setup(x => x.SendRecordToQueue(It.IsAny<ActivityResponse>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            // ACT
            Func<Task> functionAction = async () => await _sut.Run(_mockTimerInfo.Object);

            // ASSERT
            await functionAction.Should().NotThrowAsync<Exception>();
            _mockLogger.VerifyLog(logger => logger.LogInformation($"Activity Summary sent to queue."));
        }

        [Fact]
        public async Task ThrowExceptionWhenFitbitServiceFails()
        {
            // ARRANGE
            _mockFitbitService.Setup(x => x.GetActivityResponse(It.IsAny<string>())).ThrowsAsync(new Exception("Mock Failure"));

            // ACT
            Func<Task> functionAction = async () => await _sut.Run(_mockTimerInfo.Object);

            // ASSERT
            await functionAction.Should().ThrowAsync<Exception>();
            _mockLogger.VerifyLog(logger => logger.LogError($"Exception thrown in GetActivitySummaryByDate: Mock Failure"));
        }

        [Fact]
        public async Task ThrowExceptionWhenActivityServiceFails()
        {
            // ARRANGE
            var fixture = new Fixture();
            var activityResponse = fixture.Create<ActivityResponse>();

            _mockFitbitService.Setup(x => x.GetActivityResponse(It.IsAny<string>())).ReturnsAsync(activityResponse);
            _mockActivityService.Setup(x => x.SendRecordToQueue(It.IsAny<ActivityResponse>(), It.IsAny<string>())).ThrowsAsync(new Exception("Mock Failure"));

            // ACT
            Func<Task> functionAction = async () => await _sut.Run(_mockTimerInfo.Object);

            // ASSERT
            await functionAction.Should().ThrowAsync<Exception>();
            _mockLogger.VerifyLog(logger => logger.LogError($"Exception thrown in GetActivitySummaryByDate: Mock Failure"));
        }
    }
}