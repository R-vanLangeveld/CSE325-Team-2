using HolidayPlanner.Model;
using Microsoft.EntityFrameworkCore;


namespace HolidayPlanner.Data
{
    public class HolidayPlannerContext(IConfiguration configuration) : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = configuration["DATABASE_URL"] ?? configuration.GetConnectionString("Default");
            
            
            optionsBuilder.UseNpgsql(connectionString);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<EventTask> EventTasks { get; set; }
    
    }
}