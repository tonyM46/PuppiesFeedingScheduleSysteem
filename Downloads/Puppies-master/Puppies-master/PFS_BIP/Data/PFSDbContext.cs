using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PFS_BIP.Models;

namespace PFS_BIP.Data
{
    public class PFSDbContext : IdentityDbContext
    {
        public PFSDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Puppies> Puppies { get; set; }
        public DbSet<FeedingSchedule> FeedingSchedules { get; set; }
        public DbSet<HistoricalFeedingSchedule> HistoricalFeedingSchedules { get; set; }

        public DbSet<FeedingTime> FeedingTimes { get; set; }
       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          
            modelBuilder.Entity<FeedingSchedule>()
                .HasOne(fs => fs.Puppy)
                .WithMany(p => p.FeedingSchedules)
                .HasForeignKey(fs => fs.PuppyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FeedingTime>()
                .HasOne(ft => ft.FeedingSchedule)
                .WithMany(fs => fs.FeedingTimes)
                .HasForeignKey(ft => ft.FeedingScheduleId);

            modelBuilder.Entity<HistoricalFeedingSchedule>()
                .HasOne(h => h.Puppy)
                .WithMany(p => p.HistoricalFeedingSchedules)
                .HasForeignKey(h => h.PuppyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<IdentityUserLogin<string>>()
                .HasKey(l => new { l.LoginProvider, l.ProviderKey });

            base.OnModelCreating(modelBuilder);
        }
    }
}
