using Healthtrackr.Api.Activity.Common.Models;

namespace Healthtrackr.Api.Activity.Repository.Interfaces
{
    public interface ICosmosDbRepository
    {
        Task<IEnumerable<ActivityEnvelope>> GetActivityEnvelopeByDate(string date);
    }
}
