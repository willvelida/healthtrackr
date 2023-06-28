using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using Healthtrackr.Api.Activity.Common.Models;
using Healthtrackr.Api.Activity.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace Healthtrackr.Api.Activity.Services.UnitTests
{
    public class ActivityServiceShould
    {
        private Mock<ICosmosDbRepository> _cosmosRepoMock;
        private Mock<ILogger<ActivityService>> _loggerMock;

        private ActivityService _activityServiceSut;

        public ActivityServiceShould()
        {
            _cosmosRepoMock = new Mock<ICosmosDbRepository>();
            _loggerMock = new Mock<ILogger<ActivityService>>();

            _activityServiceSut = new ActivityService(_cosmosRepoMock.Object, _loggerMock.Object);
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