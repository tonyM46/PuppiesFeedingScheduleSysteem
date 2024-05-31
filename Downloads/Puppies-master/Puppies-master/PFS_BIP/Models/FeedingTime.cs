using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PFS_BIP.Models
{
    public class FeedingTime
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select a feeding schedule.")]
        [ForeignKey("FeedingSchedule")]
        public int? FeedingScheduleId { get; set; }

        [Required(ErrorMessage = "Please enter a start time.")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "Please enter an end time.")]
        public DateTime EndTime { get; set; }

        public FeedingSchedule? FeedingSchedule { get; set; }

        
    }
}
