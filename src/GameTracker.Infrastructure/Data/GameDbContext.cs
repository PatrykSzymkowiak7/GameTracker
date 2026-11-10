using GameTracker.Application.Interfaces;
using GameTracker.Domain.Entities;
using GameTracker.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GameTracker.Infrastructure.Data
{
    public class GameDbContext 
        : IdentityDbContext<
            ApplicationUser, 
            IdentityRole<int>, 
            int>
    {
        public GameDbContext(DbContextOptions<GameDbContext> options)
            : base(options)
        {

        }

        public DbSet<Game> Games => Set<Game>();
        public DbSet<Developer> Developers => Set<Developer>();
        public DbSet<Genre> Genres => Set<Genre>();
        public DbSet<GamePlatform> Platforms => Set<GamePlatform>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(GameDbContext).Assembly);
        }
    }
}
