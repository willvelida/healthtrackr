using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Healthtrackr.Activity.Common.Models
{
    public class ActivitySummaryRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CaloriesEstimationMu { get; set; }
        public int CaloriesBMR { get; set; }
        public int CaloriesOut { get; set; }
        public int ActivityCalories { get; set; }
        public double Elevation { get; set; }
        public int FairlyActiveMinutes { get; set; }
        public int LightlyActiveMinutes { get; set; }
        public int SedentaryMinutes { get; set; }
        public int VeryActiveMinutes { get; set; }
        public int Floors { get; set; }
        public int MarginalCalories { get; set; }
        public int RestingHeartRate { get; set; }
        public int Steps { get; set; }
        public string Date { get; set; }
        public int ActivityDistancesId { get; set; }
        public int ActivityHeartRateZonesId { get; set; }
    }
}
