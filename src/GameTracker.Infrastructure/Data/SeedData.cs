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
                    new Genre("RPG" ),
                    new Genre("Action"),
                    new Genre("Adventure"),
                    new Genre("Strategy")
                );
            }

            if(!context.Platforms.Any())
            {
                context.Platforms.AddRange(
                    new GamePlatform("PC"),
                    new GamePlatform("PS5"),
                    new GamePlatform("Xbox"),
                    new GamePlatform("Switch")
                );
            }

            if(!context.Developers.Any())
            {
                context.Developers.AddRange(
                    new Developer("FromSoftware"),
                    new Developer("CD Projekt Red"),
                    new Developer("Nexon"),
                    new Developer("SEGA"),
                    new Developer("ATLUS")
                );
            }

            await context.SaveChangesAsync();
        }
    }
}
