using AutoFixture;
using FluentAssertions;
using Healthtrackr.Activity.Common;
using Healthtrackr.Activity.Common.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Healthtrackr.Activity.Repository.UnitTests
{
    public class CosmosDbRepositoryShould
    {
        private readonly Mock<CosmosClient> _cosmosClientMock;
        private readonly Mock<ILogger<CosmosDbRepository>> _loggerMock;
        private readonly Mock<IOptions<Settings>> _optionsMock;
        private readonly Mock<Container> _containerMock;

        private CosmosDbRepository _sut;

        public CosmosDbRepositoryShould()
        {
            _cosmosClientMock = new Mock<CosmosClient>();
            _loggerMock = new Mock<ILogger<CosmosDbRepository>>();
            _optionsMock = new Mock<IOptions<Settings>>();
            _containerMock = new Mock<Container>();
            _optionsMock.Setup(x => x.Value).Returns(new Settings
            {
                DatabaseName = "test",
                ActivityContainerName = "test"
            });
            _cosmosClientMock.Setup(x => x.GetContainer("test", "test")).Returns(_containerMock.Object);
            _sut = new CosmosDbRepository(_cosmosClientMock.Object, _optionsMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task SuccessfullyCreateActivityDocument()
        {
            // ARRANGE
            var fixture = new Fixture();
            var activityEnvelope = fixture.Create<ActivityEnvelope>();
            var mockedItemResponse = new Mock<ItemResponse<ActivityEnvelope>>();

            mockedItemResponse.Setup(x => x.StatusCode)
                .Returns(System.Net.HttpStatusCode.OK);

            _containerMock.Setup(x => x.CreateItemAsync(It.IsAny<ActivityEnvelope>(), It.IsAny<PartitionKey>(), It.IsAny<ItemRequestOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockedItemResponse.Object)
                .Callback<ActivityEnvelope, PartitionKey?, RequestOptions, CancellationToken>(
                    (t, p, r, c) => activityEnvelope = t);

            // ACT
            Func<Task> repositoryAction = async () => await _sut.CreateActivityDocument(activityEnvelope);

            // ASSERT
            await repositoryAction.Should().NotThrowAsync<Exception>();
            _containerMock.Verify(x => x.CreateItemAsync(activityEnvelope, new PartitionKey(activityEnvelope.Date), It.IsAny<ItemRequestOptions>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ThrowExceptionWhenCreateItemAsyncFails()
        {
            // ARRANGE
            var fixture = new Fixture();
            var activityEnvelope = fixture.Create<ActivityEnvelope>();

            _containerMock.Setup(x => x.CreateItemAsync(It.IsAny<ActivityEnvelope>(), It.IsAny<PartitionKey>(), It.IsAny<ItemRequestOptions>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Mock Failure"));

            // ACT
            Func<Task> repositoryAction = async () => await _sut.CreateActivityDocument(activityEnvelope);

            // ASSERT
            await repositoryAction.Should().ThrowAsync<Exception>();
            _loggerMock.VerifyLog(logger => logger.LogError($"Exception thrown in CreateActivityDocument: Mock Failure"));
        }
    }
}