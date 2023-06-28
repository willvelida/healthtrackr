using Healthtrackr.Api.Activity.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Healthtrackr.Api.Activity.Repository.Interfaces
{
    public interface IActivityRepository
    {
        Task<List<ActivityRecord>> GetActivityRecordsByDate(string date);
        Task<ActivitySummaryRecord> GetActivitySummaryRecordByDate(string date);
    }
}
