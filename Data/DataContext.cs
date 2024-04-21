using Microsoft.EntityFrameworkCore;
using LocalEdu_App.Models;

namespace LocalEdu_App.Data
{
    public class DataContext : DbContext
    {
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Part> Parts { get; set; }
        public DbSet<PartType> PartTypes { get; set; }
        public DbSet<UserProgress> UserProgresses { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure 1-to-many relationship between Topic and Part
            modelBuilder.Entity<Part>()
                .HasOne(p => p.Topic)
                .WithMany(t => t.Parts)
                .HasForeignKey(p => p.TopicId);

            // Configure 1-to-many relationship between Part and PartType
            modelBuilder.Entity<PartType>()
                .HasOne(pt => pt.Part)
                .WithMany(p => p.PartTypes)
                .HasForeignKey(pt => pt.PartId);

            // Configure composite primary key for UserProgress
            modelBuilder.Entity<UserProgress>()
                .HasKey(up => new { up.UserId, up.TopicId });

            // Configure foreign keys for UserProgress
            modelBuilder.Entity<UserProgress>()
                .HasOne(au => au.AppUser)
                .WithMany(up => up.UserProgresses)
                .HasForeignKey(au => au.UserId);

            modelBuilder.Entity<UserProgress>()
                .HasOne(t => t.Topic)
                .WithMany(up => up.UserProgresses)
                .HasForeignKey(t => t.TopicId);

        }
    }
}
