using Healthtrackr.Activity.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Healthtrackr.Activity.Repository
{
    public class ActivityContext : DbContext
    {
        public ActivityContext(DbContextOptions<ActivityContext> options) : base(options)
        {

        }

        public DbSet<ActivitySummaryRecord> ActivitySummary { get; set; }
        public DbSet<ActivityRecord> Activity { get; set; }
    }
}
