using AutoFixture;
using FluentAssertions;
using Healthtracker.Auth.Repository.Interfaces;
using Healthtrackr.Auth.Common.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace Healthtrackr.Auth.Services.UnitTests
{
    public class RefreshTokenServiceShould
    {
        private Mock<IKeyVaultRepository> _mockKeyVaultRepository;
        private Mock<HttpClient> _mockHttpClient;
        private Mock<ILogger<RefreshTokenService>> _mockLogger;

        private RefreshTokenService _sut;

        public RefreshTokenServiceShould()
        {
            _mockKeyVaultRepository = new Mock<IKeyVaultRepository>();
            _mockHttpClient = new Mock<HttpClient>();
            _mockLogger = new Mock<ILogger<RefreshTokenService>>();

            _sut = new RefreshTokenService(
                _mockKeyVaultRepository.Object,
                _mockHttpClient.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task CatchAndThrowExceptionWhenGetSecretFailsInRefreshTokens()
        {
            // ARRANGE
            _mockKeyVaultRepository.Setup(x => x.GetSecret(It.IsAny<string>())).ThrowsAsync(new Exception());

            // ACT
            Func<Task> refreshTokenAction = async () => await _sut.RefreshTokens();

            // ASSERT
            await refreshTokenAction.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task CatchAndThrowExceptionWhenSaveSecretFailsInSaveTokens()
        {
            // ARRANGE
            var fixture = new Fixture();
            var mockTokenResponse = fixture.Create<RefreshTokenResponse>();
            _mockKeyVaultRepository.Setup(x => x.SaveSecret(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());

            // ACT
            Func<Task> refreshTokenAction = async () => await _sut.SaveTokens(mockTokenResponse);

            // ASSERT
            await refreshTokenAction.Should().ThrowAsync<Exception>();
        }
    }
}