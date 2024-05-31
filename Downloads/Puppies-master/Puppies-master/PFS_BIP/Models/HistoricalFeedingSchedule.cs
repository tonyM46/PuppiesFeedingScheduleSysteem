namespace PFS_BIP.Models
{
    public class HistoricalFeedingSchedule
    {
        public int Id { get; set; }
        public int? PuppyId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int NumberOfMeals { get; set; }

        public Puppies? Puppy { get; set; }
    }
}
