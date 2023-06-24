using Healthtrackr.Activity.Common.FitbitResponses;

namespace Healthtrackr.Activity.Services.Interfaces
{
    public interface IActivityService
    {
        Task MapActivityEnvelopeAndSaveToDatabase(string date, ActivityResponse activityResponse);
        Task SendRecordToQueue<T>(T record, string queueName);
        bool IsDateValid(string date);
    }
}
