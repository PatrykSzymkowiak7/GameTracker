using GameTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using GameTracker.Application.Interfaces;
using GameTracker.Application.Services;
using GameTracker.Infrastructure.Repositories;
using GameTracker.Application.Mapping;
using AutoMapper;
using System.Text.Json.Serialization;
using GameTracker.Api.ExceptionHandling;
using GameTracker.Api.Extensions;

const string MigrationAssembly = "GameTracker.Infrastructure";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton(provider =>
{
    var loggerFactory = provider.GetRequiredService<ILoggerFactory>();

    return new MapperConfiguration(cfg =>
    {
        cfg.AddProfile<GameProfile>();
    }, loggerFactory);
});

builder.Services.AddSingleton(provider =>
{
    var config = provider.GetRequiredService<MapperConfiguration>();

    return config.CreateMapper();
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<GameDbContext>(options =>
    options.UseSqlite(
        connectionString,
        b => b.MigrationsAssembly(MigrationAssembly)));

builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IGameService, GameService>();

var app = builder.Build();

using(var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<GameDbContext>();

    dbContext.Database.Migrate();
}

app.UseExceptionHandler();

app.UseSwagger();
app.UseSwaggerUI();

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program
{
}
