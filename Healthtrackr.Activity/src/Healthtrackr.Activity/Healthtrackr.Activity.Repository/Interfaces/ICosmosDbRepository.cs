using Healthtrackr.Activity.Common.Models;

namespace Healthtrackr.Activity.Repository.Interfaces
{
    public interface ICosmosDbRepository
    {
        Task CreateActivityDocument(ActivityEnvelope activityEnvelope);
    }
}
