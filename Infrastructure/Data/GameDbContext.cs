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
        public DbSet<Developer> Developers => Set<Developer>();
        public DbSet<Genre> Genres => Set<Genre>();
        public DbSet<GamePlatform> Platforms => Set<GamePlatform>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Game>()
                .Property(g => g.Status)
                .HasConversion<string>()
                .IsRequired(true);

            modelBuilder.Entity<Game>()
                .HasIndex(g => g.Title)
                .IsUnique();

            modelBuilder.Entity<Game>()
                .HasOne(g => g.Developer)
                .WithMany(d => d.Games)
                .HasForeignKey(g => g.DeveloperId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Game>()
                .HasMany(game => game.Genres)
                .WithMany(genre => genre.Games)
                .UsingEntity(j => j.ToTable("GameGenres"));

            modelBuilder.Entity<Game>()
                .HasMany(g => g.Platforms)
                .WithMany(g => g.Games)
                .UsingEntity(j => j.ToTable("GamePlatforms"));

            modelBuilder.Entity<Developer>()
                .HasIndex(g => g.Name)
                .IsUnique();

            modelBuilder.Entity<GamePlatform>()
                .HasIndex(p => p.Name)
                .IsUnique();
        }
    }
}
