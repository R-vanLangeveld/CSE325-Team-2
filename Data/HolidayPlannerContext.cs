using CSE325_Team_2.Model;
using Microsoft.EntityFrameworkCore;

namespace CSE325_Team_2.Data
{   
    public class CSE325_Team_2Context(IConfiguration configuration) : DbContext
    {   
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = configuration["DATABASE_URL"] ?? configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection string 'Default' not found.");
                optionsBuilder.UseNpgsql(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Plan.Creator → Users (many Plans can have the same creator)
            modelBuilder.Entity<Plan>()
                .HasOne(p => p.Creator)
                .WithMany()
                .HasForeignKey("CreatorId")
                .OnDelete(DeleteBehavior.Cascade);

            // Plan.Collaborators -> Users (one-to-many)
            modelBuilder.Entity<Plan>()
                .HasMany(p => p.Collaborators)
                .WithOne()
                .HasForeignKey("PlanId");

            // EventTask.Plan → Plans
            modelBuilder.Entity<EventTask>()
                .HasOne(e => e.Plan)
                .WithMany()
                .HasForeignKey("PlanId")
                .OnDelete(DeleteBehavior.Cascade);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<EventTask> EventTasks { get; set; }
    }
}