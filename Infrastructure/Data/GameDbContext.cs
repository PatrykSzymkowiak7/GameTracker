using GameTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameTracker.Infrastructure.Data
{
    public class GameDbContext : DbContext
    {
        public GameDbContext(DbContextOptions<GameDbContext> options)
            : base(options)
        {
            
        }

        public DbSet<Game> Games => Set<Game>();
    }
}
