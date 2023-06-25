using Healthtrackr.Activity.Common.FitbitResponses;
using Healthtrackr.Activity.Common.Models;

namespace Healthtrackr.Activity.Services.Interfaces
{
    public interface IActivityService
    {
        bool IsDateValid(string date);
        Task MapActivityEnvelopeAndSaveToDatabase(string date, ActivityResponse activityResponse);
        Task MapAndSaveActivityHeartRateRecord(ActivityEnvelope activityEnvelope);
        Task MapAndSaveActivityDistanceRecord(ActivityEnvelope activityEnvelope);
        Task MapAndSaveActivitySummaryRecord(ActivityEnvelope activityEnvelope);
        Task MapAndSaveActivityRecords(ActivityEnvelope activityEnvelope);
        Task SendRecordToQueue<T>(T record, string queueName);
    }
}
