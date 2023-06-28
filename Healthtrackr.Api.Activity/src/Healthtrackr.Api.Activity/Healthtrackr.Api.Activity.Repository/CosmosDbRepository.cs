using Healthtrackr.Api.Activity.Common;
using Healthtrackr.Api.Activity.Common.Models;
using Healthtrackr.Api.Activity.Repository.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Healthtrackr.Api.Activity.Repository
{
    public class CosmosDbRepository : ICosmosDbRepository
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Container _activityContainer;
        private readonly Settings _settings;
        private readonly ILogger<CosmosDbRepository> _logger;

        public CosmosDbRepository(CosmosClient cosmosClient, IOptions<Settings> options, ILogger<CosmosDbRepository> logger)
        {
            _settings = options.Value;
            _cosmosClient = cosmosClient;
            _activityContainer = _cosmosClient.GetContainer(_settings.DatabaseName, _settings.ActivityContainerName);
            //_activityContainer = _cosmosClient.GetContainer("HealthTrackrDB", "Activity");
            _logger = logger;
        }

        public async Task<IEnumerable<ActivityEnvelope>> GetActivityEnvelopeByDate(string date)
        {
            try
            {
                List<ActivityEnvelope> activityEnvelopes = new List<ActivityEnvelope>();
                QueryDefinition queryDefinition = new QueryDefinition("SELECT * FROM c WHERE c.Date = @date")
                    .WithParameter("@date", date);
                QueryRequestOptions queryRequestOptions = new QueryRequestOptions
                {
                    PartitionKey = new PartitionKey(date)
                };

                using (FeedIterator<ActivityEnvelope> feedIterator = _activityContainer.GetItemQueryIterator<ActivityEnvelope>(
                    queryDefinition,
                    null,
                    queryRequestOptions))
                {
                    while (feedIterator.HasMoreResults)
                    {
                        foreach (var item in await feedIterator.ReadNextAsync())
                        {
                            activityEnvelopes.Add(item);
                        }
                    }
                }

                return activityEnvelopes;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetActivityEnvelopeByDate)}: {ex.Message}");
                throw;
            }
        }
    }
}