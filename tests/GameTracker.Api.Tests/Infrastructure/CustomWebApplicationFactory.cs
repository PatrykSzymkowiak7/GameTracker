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
                UserName = "testuserA",
                Email = "testuserA@example.com",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(testUser, "Test123!");

            if(!result.Succeeded)
            {
                var errors = string.Join("; ",
                    result.Errors.Select(error => error.Description));

                throw new InvalidOperationException($"Could not create test user: {errors}");
            }

            var rpg = new Genre("RPG");
            var pc = new GamePlatform("PC");

            var fromSoftware = new Developer("FromSoftware");
            var nexon = new Developer("Nexon");

            dbContext.Games.AddRange(
                new Game(
                    title: "Elden Ring",
                    genres: [rpg],
                    platforms: [pc],
                    developer: fromSoftware,
                    status: GameStatus.Playing,
                    rating: 10,
                    hoursPlayed: 100,
                    userId: testUser.Id
                ),
                new Game(
                    title: "The First Berserker: Khazan",
                    genres: [rpg],
                    platforms: [pc],
                    developer: nexon,
                    status: GameStatus.Completed,
                    rating: 9,
                    hoursPlayed: 50,
                    userId: testUser.Id
                ));

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
