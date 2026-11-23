using GameTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using GameTracker.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using GameTracker.Api.Extensions;
using GameTracker.Infrastructure.Extensions;
using GameTracker.Application.Extensions;

const string frontendPolicy = "FrontendPolicy";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddApi(builder.Configuration)
    .AddInfrastructure(builder.Configuration)
    .AddApplication();

builder.Services.AddCors(options =>
{
    options.AddPolicy(frontendPolicy, policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

using(var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<GameDbContext>();

    dbContext.Database.Migrate();

    await SeedData.SeedAsync(dbContext);
}

app.UseExceptionHandler();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors(frontendPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

app.MapControllers();

app.Run();

public partial class Program
{
}
