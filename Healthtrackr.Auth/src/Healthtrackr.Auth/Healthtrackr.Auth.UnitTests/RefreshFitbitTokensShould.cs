using AutoFixture;
using FluentAssertions;
using Healthtrackr.Auth.Common.Models;
using Healthtrackr.Auth.Functions;
using Healthtrackr.Auth.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Moq;

namespace Healthtrackr.Auth.UnitTests
{
    public class RefreshFitbitTokensShould
    {
        private Mock<IRefreshTokenService> _mockRefreshTokenService;
        private Mock<ILogger<RefreshFitbitTokens>> _mockLogger;
        private Mock<TimerInfo> _mockTimerInfo;

        private RefreshFitbitTokens _sut;

        public RefreshFitbitTokensShould()
        {
            _mockRefreshTokenService = new Mock<IRefreshTokenService>();
            _mockLogger = new Mock<ILogger<RefreshFitbitTokens>>();
            _mockTimerInfo = new Mock<TimerInfo>();
            _sut = new RefreshFitbitTokens(_mockRefreshTokenService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task SuccessfullyRefreshTokens()
        {
            // ARRANGE
            var fixture = new Fixture();
            var testRefreshTokenResponse = fixture.Create<RefreshTokenResponse>();

            _mockRefreshTokenService.Setup(x => x.RefreshTokens()).ReturnsAsync(testRefreshTokenResponse);

            // ACT
            Func<Task> refreshTokenAction = async () => await _sut.Run(_mockTimerInfo.Object);

            // ASSERT
            await refreshTokenAction.Should().NotThrowAsync<Exception>();
        }

        [Fact]
        public async Task ThrowExceptionWhenFunctionFails()
        {
            // ARRANGE
            _mockRefreshTokenService.Setup(x => x.RefreshTokens()).ThrowsAsync(new Exception());

            // ACT
            Func<Task> refreshTokenAction = async () => await _sut.Run(_mockTimerInfo.Object);

            // ASSERT
            await refreshTokenAction.Should().ThrowAsync<Exception>();
        }
    }
}