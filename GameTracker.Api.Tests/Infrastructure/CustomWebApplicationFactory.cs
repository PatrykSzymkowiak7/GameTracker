using GameTracker.Domain.Entities;
using GameTracker.Domain.Enums;
using GameTracker.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GameTracker.Api.Tests.Infrastructure
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly SqliteConnection _connection;

        public CustomWebApplicationFactory()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DbContextOptions<GameDbContext>>();

                services.AddDbContext<GameDbContext>(options =>
                {
                    options.UseSqlite(_connection);
                });

                using var serviceProvider = services.BuildServiceProvider();

                using var scope = serviceProvider.CreateScope();

                var dbContext = scope.ServiceProvider
                .GetRequiredService<GameDbContext>();

                dbContext.Database.EnsureCreated();

                SeedDatabase(serviceProvider).GetAwaiter().GetResult();
            });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _connection.Dispose();

            base.Dispose(disposing);
        }

        private static async Task SeedDatabase(IServiceProvider services)
        {
            var dbContext = services.GetRequiredService<GameDbContext>();

            dbContext.Games.AddRange(
                new Game
                {
                    Title = "Elden Ring",
                    Genre = "RPG",
                    Platform = Platform.PC,
                    Status = GameStatus.Playing,
                    Rating = 10,
                    HoursPlayed = 100
                },
                new Game
                {
                    Title = "The First Berserker: Khazan",
                    Genre = "RPG",
                    Platform = Platform.PC,
                    Status = GameStatus.Completed,
                    Rating = 9,
                    HoursPlayed = 50
                });

            await dbContext.SaveChangesAsync();
        }

        public async Task ResetDatabaseAsync()
        {
            using var scope = Services.CreateScope();

            var dbContext = scope.ServiceProvider
                .GetRequiredService<GameDbContext>();

            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.EnsureCreatedAsync();

            await SeedDatabase(scope.ServiceProvider);
        }
    }
}
