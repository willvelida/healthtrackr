using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Healthtrackr.Activity.Common.Models
{
    public class ActivityDistancesRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ActivityType { get; set; }
        public double Distance { get; set; }
        public string Date { get; set; }
    }
}
