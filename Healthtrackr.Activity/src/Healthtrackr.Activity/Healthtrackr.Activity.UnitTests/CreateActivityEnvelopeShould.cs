using AutoFixture;
using FluentAssertions;
using Healthtrackr.Activity.Common.FitbitResponses;
using Healthtrackr.Activity.Functions;
using Healthtrackr.Activity.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;

namespace Healthtrackr.Activity.UnitTests
{
    public class CreateActivityEnvelopeShould
    {
        private readonly Mock<IActivityService> _mockActivityService;
        private readonly Mock<ILogger<CreateActivityEnvelope>> _mockLogger;

        private CreateActivityEnvelope _sut;

        public CreateActivityEnvelopeShould()
        {
            _mockActivityService = new Mock<IActivityService>();
            _mockLogger = new Mock<ILogger<CreateActivityEnvelope>>();

            _sut = new CreateActivityEnvelope(_mockActivityService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task SuccessfullyCreateActivityEnvelope()
        {
            // ARRANGE
            var fixture = new Fixture();
            var activityResponse = fixture.Create<ActivityResponse>();
            var activityResponseJson = JsonConvert.SerializeObject(activityResponse);

            _mockActivityService.Setup(x => x.MapActivityEnvelopeAndSaveToDatabase(It.IsAny<string>(), It.IsAny<ActivityResponse>())).Returns(Task.CompletedTask);

            // ACT
            Func<Task> functionAction = async () => await _sut.RunAsync(activityResponseJson);

            // ASSERT
            await functionAction.Should().NotThrowAsync<Exception>();
        }

        [Fact]
        public async Task ThrowExceptionWhenActivitySerivceFails()
        {
            // ARRANGE
            var fixture = new Fixture();
            var activityResponse = fixture.Create<ActivityResponse>();
            var activityResponseJson = JsonConvert.SerializeObject(activityResponse);

            _mockActivityService.Setup(x => x.MapActivityEnvelopeAndSaveToDatabase(It.IsAny<string>(), It.IsAny<ActivityResponse>())).ThrowsAsync(new Exception("Mock Failure"));

            // ACT
            Func<Task> functionAction = async () => await _sut.RunAsync(activityResponseJson);

            // ASSERT
            await functionAction.Should().ThrowAsync<Exception>();
            _mockLogger.VerifyLog(logger => logger.LogError($"Exception thrown in CreateActivityEnvelope: Mock Failure"));
        }
    }
}
