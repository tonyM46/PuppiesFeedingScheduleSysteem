namespace PFS_BIP.JSONEntities
{
    public class JSONFeedingSchedule
    {
        public int PuppyId { get; set; }

        public short NumberOfMeals { get; set; }
        public List<JSONFeedingTime> FeedingTimes { get; set; } = new List<JSONFeedingTime>();
      
    }
    public class JSONFeedingTime
    {
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}