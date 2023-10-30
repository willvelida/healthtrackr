using AutoFixture;
using Azure;
using Azure.Security.KeyVault.Secrets;
using FluentAssertions;
using Healthtrackr.Activity.Common.FitbitResponses;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System.Net;

namespace Healthtrackr.Activity.Services.UnitTests
{
    public class FitbitServiceShould
    {
        private Mock<SecretClient> _secretClientMock;
        private readonly Mock<ILogger<FitbitService>> _loggerMock;
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly Mock<HttpClient> _httpClientMock;

        private FitbitService _sut;

        public FitbitServiceShould()
        {
            _secretClientMock = new Mock<SecretClient>();
            _loggerMock = new Mock<ILogger<FitbitService>>();
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClientMock = new Mock<HttpClient>(_httpMessageHandlerMock.Object);

            _sut = new FitbitService(_secretClientMock.Object, _httpClientMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task ReturnActivityResponseSuccessfully()
        {
            // ARRANGE
            var date = "2022-01-01";
            var fixture = new Fixture();
            var activityResponse = fixture.Create<ActivityResponse>();
            var mockKeyVaultSecret = fixture.Create<KeyVaultSecret>();

            _secretClientMock.Setup(x => x.GetSecretAsync("AccessToken", It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Response.FromValue(mockKeyVaultSecret, new Mock<Response>().Object));
            _httpMessageHandlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(activityResponse))
                });

            // ACT
            var result = await _sut.GetActivityResponse(date);

            // ASSERT
            Assert.IsType<ActivityResponse>(result);
        }

        [Fact]
        public async Task ThrowExceptionWhenCallToFitbitAPIFails()
        {
            // ARRANGE
            var date = "2022-01-01";
            var fixture = new Fixture();
            var mockKeyVaultSecret = fixture.Create<KeyVaultSecret>();

            _secretClientMock.Setup(x => x.GetSecretAsync("AccessToken", It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Response.FromValue(mockKeyVaultSecret, new Mock<Response>().Object));
            _httpMessageHandlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            // ACT
            Func<Task> fitbitServiceAction = () => _sut.GetActivityResponse(date);

            // ASSERT
            await fitbitServiceAction.Should().ThrowAsync<Exception>();
            _loggerMock.VerifyLog(logger => logger.LogError($"Exception thrown in GetActivityResponse: Response status code does not indicate success: 500 (Internal Server Error)."));
        }

        [Fact]
        public async Task ThrowExceptionWhenGetSecretAsyncFails()
        {
            // ARRANGE
            var date = "2022-01-01";

            _secretClientMock.Setup(x => x.GetSecretAsync("AccessToken", It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Mock Failure"));

            // ACT
            Func<Task> fitbitServiceAction = () => _sut.GetActivityResponse(date);

            // ASSERT
            await fitbitServiceAction.Should().ThrowAsync<Exception>();
            _loggerMock.VerifyLog(logger => logger.LogError($"Exception thrown in GetActivityResponse: Mock Failure"));
        }
    }
}
