using GameTracker.Application.Interfaces;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Game>()
                .Property(g => g.Platform)
                .HasConversion<string>()
                .IsRequired(true);

            modelBuilder.Entity<Game>()
                .Property(g => g.Status)
                .HasConversion<string>()
                .IsRequired(true);
        }
    }
}
