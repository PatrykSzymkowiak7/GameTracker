using GameTracker.Domain.Entities;
using GameTracker.Domain.Enums;
using GameTracker.Infrastructure.Authentication;
using GameTracker.Infrastructure.Data;
using GameTracker.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace GameTracker.Api.Tests.Infrastructure
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly SqliteConnection _connection;

        private const string TestJwtKey = "GameTracker_Test_Only_Jwt_Key_2026_AtLeast32Chars!";
        private const string TestJwtIssuer = "GameTracker";
        private const string TestJwtAudience = "GameTracker.Api";

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

                services.PostConfigure<JwtOptions>(options =>
                {
                    options.Key = TestJwtKey;
                    options.Issuer = TestJwtIssuer;
                    options.Audience = TestJwtAudience;
                    options.ExpirationMinutes = 60;
                });

                services.PostConfigure<JwtBearerOptions>(
                JwtBearerDefaults.AuthenticationScheme,
                options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = TestJwtIssuer,

                        ValidateAudience = true,
                        ValidAudience = TestJwtAudience,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(TestJwtKey)),

                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });
            });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
                _connection.Dispose();
        }

        private static async Task SeedDatabase(IServiceProvider services)
        {
            var dbContext = services.GetRequiredService<GameDbContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            var testUser = new ApplicationUser()
            {
                UserName = "testuser",
                Email = "testuser@example.com",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(testUser, "Test123!");

            if(!result.Succeeded)
            {
                var errors = string.Join("; ",
                    result.Errors.Select(error => error.Description));

                throw new InvalidOperationException($"Could not create test user: {errors}");
            }

            var rpg = new Genre(){ Name = "RPG" };
            var pc = new GamePlatform() { Name = "PC" };

            var fromSoftware = new Developer { Name = "FromSoftware"};
            var nexon = new Developer { Name = "Nexon" };

            dbContext.Games.AddRange(
                new Game
                {
                    Title = "Elden Ring",
                    Genres = [rpg],
                    Platforms = [pc],
                    Developer = fromSoftware,
                    Status = GameStatus.Playing,
                    Rating = 10,
                    HoursPlayed = 100,
                    UserId = testUser.Id
                },
                new Game
                {
                    Title = "The First Berserker: Khazan",
                    Genres = [rpg],
                    Platforms = [pc],
                    Developer = nexon,
                    Status = GameStatus.Completed,
                    Rating = 9,
                    HoursPlayed = 50,
                    UserId = testUser.Id
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
