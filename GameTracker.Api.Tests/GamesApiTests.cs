using GameTracker.Api.Tests.Infrastructure;
using GameTracker.Application.DTOs;
using GameTracker.Domain.Enums;
using GameTracker.Infrastructure.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace GameTracker.Api.Tests
{
    public class GamesApiTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        private static readonly JsonSerializerOptions JsonOptions = CreateJsonOptions();

        private static readonly ApplicationUser testUser = new ApplicationUser()
        {
            Id = 1,
            UserName = "TestUser",
            Email = "testuser@example.com",
            EmailConfirmed = true
        };

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

        private async Task CreateAuthenticatedUserAsync(HttpClient client)
        {
            var loginResponse = await client.PostAsJsonAsync(
                "/api/auth/login",
                new LoginDto
                {
                    Username = "testuser",
                    Password = "Test123!"
                });

            loginResponse.EnsureSuccessStatusCode();

            var authResponse = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
            Assert.NotNull(authResponse);

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    authResponse.AccessToken);
        }

        [Fact]
        public async Task GetGames_ReturnsSuccessStatusCode()
        {
            await _factory.ResetDatabaseAsync();

            var client = _factory.CreateClient();

            await CreateAuthenticatedUserAsync(client);

            var response = await client.GetAsync("/api/v1/Games");

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        private async Task GetGames_ReturnsSeededGames()
        {
            await _factory.ResetDatabaseAsync();

            var client = _factory.CreateClient();

            await CreateAuthenticatedUserAsync(client);

            var response = await client.GetAsync("api/v1/Games");

            var responseBody = await response.Content.ReadAsStringAsync();
            var authHeader = string.Join(
                " | ",
                response.Headers.WwwAuthenticate.Select(h => h.ToString()));

            Assert.True(
                response.IsSuccessStatusCode,
                $"Status: {(int)response.StatusCode} ({response.StatusCode})" +
                $"\nResponse: {responseBody}" +
                $"\nWWW-Authenticate: {authHeader}");

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

            await CreateAuthenticatedUserAsync(client);

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
                "api/v1/Games",
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

            await CreateAuthenticatedUserAsync(client);

            var request = new CreateGameDto
            {
                Title = "Elden Ring",
                GenreIds = new List<int> { 1 },
                PlatformIds = new List<int> { 1 },
                Status = GameStatus.Backlog,
            };

            var response = await client.PostAsJsonAsync(
                "api/v1/Games",
                request);

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task DeleteGame_WithId_ReturnsNoContent()
        {
            await _factory.ResetDatabaseAsync();

            var client = _factory.CreateClient();

            await CreateAuthenticatedUserAsync(client);

            var response = await client.DeleteAsync("api/v1/Games/1");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            response = await client.GetAsync("api/v1/Games/1");

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateGame_WithValidData_ReturnsUpdatedGame()
        {
            await _factory.ResetDatabaseAsync();

            var client = _factory.CreateClient();

            await CreateAuthenticatedUserAsync(client);

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
                "api/v1/Games/1",
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

            await CreateAuthenticatedUserAsync(client);

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
                "api/v1/Games/999",
                request);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteGame_WHenGameDoesNotExist_ReturnsNotFound()
        {
            await _factory.ResetDatabaseAsync();

            var client = _factory.CreateClient();

            await CreateAuthenticatedUserAsync(client);

            var response = await client.DeleteAsync("api/v1/Games/999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreateGame_WithInvalidData_ReturnsBadRequest()
        {
            await _factory.ResetDatabaseAsync();

            var client = _factory.CreateClient();

            await CreateAuthenticatedUserAsync(client);

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
                "/api/v1/Games",
                request,
                JsonOptions);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
