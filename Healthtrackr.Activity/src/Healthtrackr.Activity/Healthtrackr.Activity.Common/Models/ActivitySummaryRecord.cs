using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Healthtrackr.Activity.Common.Models
{
    public class ActivitySummaryRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Date { get; set; }
        public int CaloriesBurned { get; set; }
        public int Steps { get; set; }
        public double Distance { get; set; }
        public int Floors { get; set; }
        public int MinutesSedentary { get; set; }
        public int MinutesLightlyActive { get; set; }
        public int MinutesFairlyActive { get; set; }
        public int MinutesVeryActive { get; set; }
        public int ActivityCalories { get; set; }
    }
}
