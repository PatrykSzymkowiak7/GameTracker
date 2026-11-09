using GameTracker.Application.Interfaces;
using GameTracker.Infrastructure.Authentication;
using GameTracker.Infrastructure.Data;
using GameTracker.Infrastructure.Identity;
using GameTracker.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GameTracker.Infrastructure.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Connection string is missing.");

            services.AddDbContext<GameDbContext>(options =>
                options.UseSqlite(
                    connectionString,
                    b => b.MigrationsAssembly(
                        typeof(GameDbContext).Assembly.FullName)));

            services.AddScoped<IGameRepository, GameRepository>();

            services
                .AddIdentityCore<ApplicationUser>()
                .AddRoles<IdentityRole<int>>()
                .AddEntityFrameworkStores<GameDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.User.RequireUniqueEmail = true;

                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
            });

            services
                .AddHealthChecks()
                .AddDbContextCheck<GameDbContext>(
                    name: "sqlite",
                    tags: new[] { "ready" });

            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
