namespace Healthtrackr.Api.Activity.Common.Models
{
    public class ActivityRecord
    {
        public int Id { get; set; }
        public string? ActivityName { get; set; }
        public int Calories { get; set; }
        public int Duration { get; set; }
        public int Steps { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
    }
}
