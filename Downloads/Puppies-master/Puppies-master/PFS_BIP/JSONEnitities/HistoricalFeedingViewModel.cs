using PFS_BIP.Models;

public class HistoricalFeedingViewModel
{

    public int? PuppyId { get; set; }
   
    public List<HistoricalFeedingSchedule> HistoricalSchedules { get; set; }=new List<HistoricalFeedingSchedule>();
    public string PuppyName { get; set; }
}