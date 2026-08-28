using GameTracker.Api.Tests.Infrastructure;
using GameTracker.Application.DTOs;
using GameTracker.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;

namespace GameTracker.Api.Tests
{
    public class GamesApiTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        private static readonly JsonSerializerOptions JsonOptions = CreateJsonOptions();

        private static JsonSerializerOptions CreateJsonOptions()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            options.Converters.Add(new JsonStringEnumConverter());

            return options;
        }

        public GamesApiTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetGames_ReturnsSuccessStatusCode()
        {
            await _factory.ResetDatabaseAsync();

            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/Games");

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        private async Task GetGames_ReturnsSeededGames()
        {
            await _factory.ResetDatabaseAsync();

            var client = _factory.CreateClient();

            var response = await client.GetAsync("api/Games");

            response.EnsureSuccessStatusCode();

            var result = await response.Content
                .ReadFromJsonAsync<PagedResultDto<GameDto>>(JsonOptions);

            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Items.Count());
        }

        [Fact]
        public async Task CreateGame_WithValidData_ReturnsCreated()
        {
            await _factory.ResetDatabaseAsync();

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            jsonOptions.Converters.Add(new JsonStringEnumConverter());

            var client = _factory.CreateClient();

            var request = new CreateGameDto
            {
                Title = "Cyberpunk 2077",
                GenreIds = new List<int> { 1 },
                PlatformIds = new List<int> { 1 },
                Status = GameStatus.Backlog,
                Rating = 9,
                HoursPlayed = 0,
            };

            var response = await client.PostAsJsonAsync(
                "api/Games",
                request,
                jsonOptions);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var createdGame = await response.Content
                .ReadFromJsonAsync<GameDto>(jsonOptions);

            Assert.NotNull(createdGame);
            Assert.Equal(request.Title, createdGame.Title);
        }

        [Fact]
        public async Task CreateGame_WithExistingTItle_ReturnsConflict()
        {
            await _factory.ResetDatabaseAsync();

            var client = _factory.CreateClient();

            var request = new CreateGameDto
            {
                Title = "Elden Ring",
                GenreIds = new List<int> { 1 },
                PlatformIds = new List<int> { 1 },
                Status = GameStatus.Backlog
            };

            var response = await client.PostAsJsonAsync(
                "api/Games",
                request);

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task DeleteGame_WithId_ReturnsNoContent()
        {
            await _factory.ResetDatabaseAsync();

            var client = _factory.CreateClient();

            var response = await client.DeleteAsync("api/Games/1");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            response = await client.GetAsync("api/Games/1");

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateGame_WithValidData_ReturnsUpdatedGame()
        {
            await _factory.ResetDatabaseAsync();

            var client = _factory.CreateClient();

            var request = new UpdateGameDto
            {
                Title = "Elden Ring",
                Genre = "RPG",
                Platform = Platform.PC,
                Status = GameStatus.Completed,
                Rating = 10,
                HoursPlayed = 150
            };

            var response = await client.PutAsJsonAsync(
                "api/Games/1",
                request,
                JsonOptions);

            var updatedGame = await response.Content
                .ReadFromJsonAsync<GameDto>(JsonOptions);

            Assert.NotNull(updatedGame);
            Assert.Equal(GameStatus.Completed, updatedGame.Status);
            Assert.Equal(150, updatedGame.HoursPlayed);
        }

        [Fact]
        public async Task UpdateGame_WhenGameDoesNotExist_ReturnsNotFound()
        {
            await _factory.ResetDatabaseAsync();

            var client = _factory.CreateClient();

            var request = new UpdateGameDto
            {
                Title = "Unknown Game",
                Genre = "RPG",
                Platform = Platform.PC,
                Status = GameStatus.Backlog,
                Rating = 5,
                HoursPlayed = 0
            };

            var response = await client.PutAsJsonAsync(
                "api/Games/999",
                request);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteGame_WHenGameDoesNotExist_ReturnsNotFound()
        {
            await _factory.ResetDatabaseAsync();

            var client = _factory.CreateClient();

            var response = await client.DeleteAsync("api/Games/999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreateGame_WithInvalidData_ReturnsBadRequest()
        {
            await _factory.ResetDatabaseAsync();

            var client = _factory.CreateClient();

            var request = new CreateGameDto
            {
                Title = "",
                GenreIds = new List<int> { 1 },
                PlatformIds = new List<int> { 1 },
                Status = GameStatus.Backlog,
                Rating = 15,
                HoursPlayed = -10
            };

            var response = await client.PostAsJsonAsync(
                "/api/Games",
                request,
                JsonOptions);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
