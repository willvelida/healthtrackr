using AutoFixture;
using FluentAssertions;
using Healthtrackr.Auth.Repository.Interfaces;
using Healthtrackr.Auth.Common.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Azure.Core;
using System.Net.Sockets;
using Azure.Security.KeyVault.Secrets;
using Moq.Protected;
using Newtonsoft.Json;

namespace Healthtrackr.Auth.Services.UnitTests
{
    public class RefreshTokenServiceShould
    {
        private Mock<IKeyVaultRepository> _mockKeyVaultRepository;
        private Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private Mock<HttpClient> _mockHttpClient;
        private Mock<ILogger<RefreshTokenService>> _mockLogger;

        private RefreshTokenService _sut;

        public RefreshTokenServiceShould()
        {
            _mockKeyVaultRepository = new Mock<IKeyVaultRepository>();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _mockHttpClient = new Mock<HttpClient>(_mockHttpMessageHandler.Object);
            _mockLogger = new Mock<ILogger<RefreshTokenService>>();

            _sut = new RefreshTokenService(
                _mockKeyVaultRepository.Object,
                _mockHttpClient.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task RefreshTokensWhenRefreshTokensIsCalled()
        {
            // ARRANGE
            var fixture = new Fixture();
            var mockRefreshTokenResponse = fixture.Create<RefreshTokenResponse>();
            var mockFitbitRefreshToken = "testFitbitRefreshToken";
            var mockFitbitCredential = "testFitbitCredential";

            _mockKeyVaultRepository.Setup(x => x.GetSecret("RefreshToken")).ReturnsAsync(new KeyVaultSecret("RefreshToken", mockFitbitRefreshToken));
            _mockKeyVaultRepository.Setup(x => x.GetSecret("FitbitCredentials")).ReturnsAsync(new KeyVaultSecret("FitbitCredentials", mockFitbitCredential));

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<System.Threading.CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(mockRefreshTokenResponse))
                });

            // ACT
            var result = await _sut.RefreshTokens();

            // ASSERT
            Assert.Equal(mockRefreshTokenResponse.AccessToken, result.AccessToken);
            Assert.Equal(mockRefreshTokenResponse.RefreshToken, result.RefreshToken);
        }

        [Fact]
        public async Task SaveRefreshTokensWhenSaveTokensIsCalled()
        {
            // ARRANGE
            var fixture = new Fixture();
            var mockTokenResponse = fixture.Create<RefreshTokenResponse>();

            _mockKeyVaultRepository.Setup(x => x.SaveSecret("RefreshToken", mockTokenResponse.RefreshToken)).Returns(Task.CompletedTask);
            _mockKeyVaultRepository.Setup(x => x.SaveSecret("AccessToken", mockTokenResponse.AccessToken)).Returns(Task.CompletedTask);

            // ACT
            Func<Task> saveTokenAction = async () => await _sut.SaveTokens(mockTokenResponse);

            // ASSERT
            await saveTokenAction.Should().NotThrowAsync<Exception>();
            _mockKeyVaultRepository.Verify(x => x.SaveSecret(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
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