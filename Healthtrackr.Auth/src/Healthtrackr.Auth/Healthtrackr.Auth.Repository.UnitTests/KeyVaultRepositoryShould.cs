using AutoFixture;
using Azure;
using Azure.Security.KeyVault.Secrets;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Healthtrackr.Auth.Repository.UnitTests
{
    public class KeyVaultRepositoryShould
    {
        private Mock<SecretClient> _secretClientMock;
        private Mock<ILogger<KeyVaultRepository>> _loggerMock;

        private KeyVaultRepository _sut;

        public KeyVaultRepositoryShould()
        {
            _secretClientMock = new Mock<SecretClient>();
            _loggerMock = new Mock<ILogger<KeyVaultRepository>>();

            _sut = new KeyVaultRepository(
                _secretClientMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task RetrieveSecretWhenCallingGetSecret()
        {
            // ARRANGE
            var fixture = new Fixture();
            var mockKeyVaultSecret = fixture.Create<KeyVaultSecret>();
            _secretClientMock.Setup(x => x.GetSecretAsync(It.IsAny<string>(), It.IsAny<string>(), new CancellationToken()))
                .ReturnsAsync(Response.FromValue(mockKeyVaultSecret, new Mock<Response>().Object));

            // ACT
            Func<Task> retrieveSecretAction = async () => await _sut.GetSecret(It.IsAny<string>());

            // ASSERT
            await retrieveSecretAction.Should().NotThrowAsync<Exception>();
        }

        [Fact]
        public async Task SaveSecretWhenCallingSaveSecret()
        {
            // ARRANGE
            var fixture = new Fixture();
            var mockKeyVaultSecret = fixture.Create<KeyVaultSecret>();
            _secretClientMock.Setup(x => x.GetSecretAsync(It.IsAny<string>(), It.IsAny<string>(), new CancellationToken()))
                .ReturnsAsync(Response.FromValue(mockKeyVaultSecret, new Mock<Response>().Object));

            // ACT
            Func<Task> saveSecretAction = async () => await _sut.SaveSecret(It.IsAny<string>(), It.IsAny<string>());

            // ASSERT
            await saveSecretAction.Should().NotThrowAsync<Exception>(); 
        }

        [Fact]
        public async Task ThrowExceptionWhenGetSecretFails()
        {
            // ARRANGE
            _secretClientMock.Setup(x => x.GetSecretAsync(It.IsAny<string>(), It.IsAny<string>(), new CancellationToken()))
                .ThrowsAsync(new Exception());

            // ACT & ASSERT
            await Assert.ThrowsAsync<Exception>(() => _sut.GetSecret(It.IsAny<string>()));
        }

        [Fact]
        public async Task ThrowExceptionWhenSaveSecretFails()
        {
            // ARRANGE
            _secretClientMock.Setup(x => x.SetSecretAsync(It.IsAny<string>(), It.IsAny<string>(), new CancellationToken()))
                .ThrowsAsync(new Exception());

            // ACT & ASSERT
            await Assert.ThrowsAsync<Exception>(() => _sut.SaveSecret(It.IsAny<string>(), It.IsAny<string>()));
        }
    }
}