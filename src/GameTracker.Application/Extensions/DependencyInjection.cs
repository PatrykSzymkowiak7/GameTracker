using AutoMapper;
using GameTracker.Application.Interfaces;
using GameTracker.Application.Mapping;
using GameTracker.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GameTracker.Application.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            services.AddScoped<IGameService, GameService>();

            // Konfiguracja mapowania, jeżeli GameProfile
            // znajduje się w projekcie Application.
            services.AddSingleton(provider =>
            {
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();

                return new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<GameProfile>();
                }, loggerFactory);
            });

            services.AddSingleton(provider =>
            {
                var config = provider.GetRequiredService<MapperConfiguration>();
                return config.CreateMapper();
            });

            return services;
        }
    }
}
