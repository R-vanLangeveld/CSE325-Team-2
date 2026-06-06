        using HolidayPlanner.Model;
        using Microsoft.EntityFrameworkCore;
        using Npgsql;


        namespace HolidayPlanner.Data
        {   


            public class HolidayPlannerContext(IConfiguration configuration) : DbContext
            {   

                
                protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                {
                    if (!optionsBuilder.IsConfigured)
                    {
                    var connectionString = configuration["DATABASE_URL"] ?? configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection string 'Default' not found.");
                    
                    optionsBuilder.UseNpgsql(connectionString);
                    // using var dataSource = NpgsqlDataSource.Create(conn  ectionString);
                    }
                }

                public DbSet<User> Users { get; set; }
                public DbSet<Plan> Plans { get; set; }
                public DbSet<EventTask> EventTasks { get; set; }
            
            }
        }