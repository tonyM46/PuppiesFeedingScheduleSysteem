using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PFS_BIP.Models
{
    public class FeedingSchedule
    {
      
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select a puppy.")]
        [ForeignKey("Puppy")]
        public int? PuppyId { get; set; }

        public int NumberOfMeals { get; set; }
        public bool IsFed { get; set; }

        public Puppies? Puppy { get; set; }

        public List<FeedingTime> FeedingTimes { get; set; } 

        public FeedingSchedule() { 
            
         FeedingTimes = new List<FeedingTime>();
        
        }
    }
}
