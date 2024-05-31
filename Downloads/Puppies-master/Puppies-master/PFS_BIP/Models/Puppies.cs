using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PFS_BIP.Models
{
    [Index(nameof(ChipNumber), IsUnique = true)]
    public class Puppies
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Age { get; set; }
        public int Weight { get; set; }

        [RegularExpression("^[0-9]{15}$", ErrorMessage = "Chip Number must be 15 digits.")]
        public string ChipNumber { get; set; }
        public bool InQuarantine { get; set; }

        [RegularExpression("^[SML]$", ErrorMessage = "Group must be 'S', 'M', or 'L' in uppercase.")]
        public string Group { get; set; }

        // Navigatie-eigenschap naar FeedingSchedule
        //public  List<FeedingSchedule> FeedingSchedules { get; set; }

        public List<FeedingSchedule> FeedingSchedules { get; set; }
        public ICollection<HistoricalFeedingSchedule>  HistoricalFeedingSchedules { get; set; }

        public Puppies()
        {
            FeedingSchedules = new List<FeedingSchedule>();
            HistoricalFeedingSchedules= new List<HistoricalFeedingSchedule>();
        }




    }
}
