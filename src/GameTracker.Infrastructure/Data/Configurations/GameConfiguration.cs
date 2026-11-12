using GameTracker.Domain.Entities;
using GameTracker.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Infrastructure.Data.Configuration
{
    public class GameConfiguration : IEntityTypeConfiguration<Game>
    {
        public void Configure(EntityTypeBuilder<Game> builder)
        {
            builder.Property(g => g.Status)
                .HasConversion<string>()
                .IsRequired(true);

            builder.HasIndex(g => new { g.Title, g.UserId })
                .IsUnique();

            builder.HasOne(g => g.Developer)
                .WithMany(d => d.Games)
                .HasForeignKey(g => g.DeveloperId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Navigation(g => g.Genres)
                .HasField("_genres")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(game => game.Genres)
                .WithMany(genre => genre.Games)
                .UsingEntity(j => j.ToTable("GameGenres"));

            builder.Navigation(g => g.Platforms)
                .HasField("_platforms")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(g => g.Platforms)
                .WithMany(g => g.Games)
                .UsingEntity(j => j.ToTable("GamePlatforms"));

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(g => g.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
