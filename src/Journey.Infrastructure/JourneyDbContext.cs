using Journey.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Journey.Infrastructure
{
    public class JourneyDbContext : DbContext
    {
        public JourneyDbContext(DbContextOptions<JourneyDbContext> options) : base(options) { }

        public DbSet<Trip> Trips { get; set; }
        public DbSet<Activity> Activities { get; set; } 
        public DbSet<User> Users { get; set; }
    }
}
