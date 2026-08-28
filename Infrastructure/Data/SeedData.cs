using GameTracker.Domain.Entities;

namespace GameTracker.Infrastructure.Data
{
    public static class SeedData
    {
        public static async Task SeedAsync(GameDbContext context)
        {
            if(!context.Genres.Any())
            {
                context.Genres.AddRange(
                    new Genre { Name = "RPG" },
                    new Genre { Name = "Action"},
                    new Genre { Name = "Adventure"},
                    new Genre { Name = "Strategy"}
                );
            }

            if(!context.Platforms.Any())
            {
                context.Platforms.AddRange(
                    new GamePlatform { Name = "PC" },
                    new GamePlatform { Name = "PS5" },
                    new GamePlatform { Name = "Xbox"},
                    new GamePlatform { Name = "Switch"}
                );
            }

            if(!context.Developers.Any())
            {
                context.Developers.AddRange(
                    new Developer { Name = "FromSoftware" },
                    new Developer { Name = "CD Projekt Red" },
                    new Developer { Name = "Nexon" },
                    new Developer { Name = "SEGA" },
                    new Developer { Name = "ATLUS" }
                );
            }

            await context.SaveChangesAsync();
        }
    }
}
