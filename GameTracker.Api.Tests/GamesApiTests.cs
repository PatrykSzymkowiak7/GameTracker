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

        private const string ApiVersion = "v1";

        private const string TestUsernameA = "testuserA";
        private const string TestUsernameB = "testuserB";
        private const string TestPassword = "Test123!";

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

        private async Task CreateAuthenticatedUserAsync(HttpClient client,
            string userName, 
            string password)
        {
            var loginResponse = await client.PostAsJsonAsync(
                "/api/auth/login",
                new LoginDto
                {
                    Username = userName,
                    Password = password
                });

            loginResponse.EnsureSuccessStatusCode();

            var authResponse = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();

            Assert.NotNull(authResponse);

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    authResponse.AccessToken);
        }

        private async Task RegisterUserAsync(HttpClient client,
            string userName,
            string password)
        {
            var registerResponse = await client.PostAsJsonAsync($"/api/auth/register",
                new RegisterDto
                {
                    Username = userName,
                    Email = $"{userName}@example.com",
                    Password = password
                });

            registerResponse.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetGames_ReturnsSuccessStatusCode()
        {
            await _factory.ResetDatabaseAsync();

            var client = _factory.CreateClient();

            await CreateAuthenticatedUserAsync(client,TestUsernameA,TestPassword);

            var response = await client.GetAsync($"/api/{ApiVersion}/Games");

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        private async Task GetGames_ReturnsSeededGames()
        {
            await _factory.ResetDatabaseAsync();

            var client = _factory.CreateClient();

            await CreateAuthenticatedUserAsync(client,TestUsernameA,TestPassword);

            var response = await client.GetAsync($"api/{ApiVersion}/Games");

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

            await CreateAuthenticatedUserAsync(client,TestUsernameA,TestPassword);

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
                $"api/{ApiVersion}/Games",
                request,
                jsonOptions);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var createdGame = await response.Content
                .ReadFromJsonAsync<GameDto>(jsonOptions);

            Assert.NotNull(createdGame);
            Assert.Equal(request.Title, createdGame.Title);
        }

        [Fact]
        public async Task CreateGame_WithExistingTitle_ReturnsConflict()
        {
            await _factory.ResetDatabaseAsync();

            var client = _factory.CreateClient();

            await CreateAuthenticatedUserAsync(client,TestUsernameA,TestPassword);

            var request = new CreateGameDto
            {
                Title = "Elden Ring",
                GenreIds = new List<int> { 1 },
                PlatformIds = new List<int> { 1 },
                Status = GameStatus.Backlog,
            };

            var response = await client.PostAsJsonAsync(
                $"api/{ApiVersion}/Games",
                request);

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task DeleteGame_WithId_ReturnsNoContent()
        {
            await _factory.ResetDatabaseAsync();

            var client = _factory.CreateClient();

            await CreateAuthenticatedUserAsync(client,TestUsernameA,TestPassword);

            var response = await client.DeleteAsync($"api/{ApiVersion}/Games/1");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            response = await client.GetAsync($"api/{ApiVersion}/Games/1");

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateGame_WithValidData_ReturnsUpdatedGame()
        {
            await _factory.ResetDatabaseAsync();

            var client = _factory.CreateClient();

            await CreateAuthenticatedUserAsync(client,TestUsernameA,TestPassword);

            var request = new UpdateGameDto
            {
                Title = "Elden Ring",
                GenreIds = new List<int> { 1 },
                PlatformIds = new List<int> { 1 },
                Status = GameStatus.Completed,
                Rating = 10,
                HoursPlayed = 150
            };

            var response = await client.PutAsJsonAsync(
                $"api/{ApiVersion}/Games/1",
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

            await CreateAuthenticatedUserAsync(client,TestUsernameA,TestPassword);

            var request = new UpdateGameDto
            {
                Title = "Unknown Game",
                GenreIds = new List<int> { 1 },
                PlatformIds = new List<int> { 1 },
                Status = GameStatus.Backlog,
                Rating = 5,
                HoursPlayed = 0
            };

            var response = await client.PutAsJsonAsync(
                $"api/{ApiVersion}/Games/999",
                request);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteGame_WhenGameDoesNotExist_ReturnsNotFound()
        {
            await _factory.ResetDatabaseAsync();

            var client = _factory.CreateClient();

            await CreateAuthenticatedUserAsync(client,TestUsernameA,TestPassword);

            var response = await client.DeleteAsync($"api/{ApiVersion}/Games/999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreateGame_WithInvalidData_ReturnsBadRequest()
        {
            await _factory.ResetDatabaseAsync();

            var client = _factory.CreateClient();

            await CreateAuthenticatedUserAsync(client,TestUsernameA,TestPassword);

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
                $"/api/{ApiVersion}/Games",
                request,
                JsonOptions);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task User_CannotAccessAnotherUsersGame()
        {
            await _factory.ResetDatabaseAsync();

            using var userAClient = _factory.CreateClient();

            // User A already exists in the Seed, no need to register
            await CreateAuthenticatedUserAsync(
                userAClient,
                TestUsernameA, 
                TestPassword);

            var ownerResponse = await userAClient.GetAsync($"/api/{ApiVersion}/Games/1");

            Assert.Equal(HttpStatusCode.OK, ownerResponse.StatusCode);

            using var userBClient = _factory.CreateClient();

            // Register new user and check if he can access a game which not belongs to him
            await RegisterUserAsync(
                userBClient, 
                TestUsernameB, 
                TestPassword);

            await CreateAuthenticatedUserAsync(
                userBClient, 
                TestUsernameB, 
                TestPassword);

            var userBResponse = await userBClient.GetAsync($"/api/{ApiVersion}/Games/1");

            Assert.Equal(HttpStatusCode.NotFound, userBResponse.StatusCode);
        }

        [Fact]
        public async Task User_CannotUpdateAnotherUsersGame()
        {
            await _factory.ResetDatabaseAsync();

            using var userBClient = _factory.CreateClient();

            await RegisterUserAsync(
                userBClient,
                TestUsernameB,
                TestPassword);

            await CreateAuthenticatedUserAsync(
                userBClient,
                TestUsernameB,
                TestPassword);

            var updateRequest = new UpdateGameDto
            {
                Title = "Modified Elden Ring",
                DeveloperId = 1,
                GenreIds = new List<int> { 1 },
                PlatformIds = new List<int> { 1 },
                Status = GameStatus.Completed,
                Rating = 10,
                HoursPlayed = 200
            };

            var response = await userBClient.PutAsJsonAsync(
                $"/api/{ApiVersion}/Games/1",
                updateRequest);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task User_CannotDeleteAnotherUserGame()
        {
            await _factory.ResetDatabaseAsync();

            using var userAClient = _factory.CreateClient();

            await CreateAuthenticatedUserAsync(
                userAClient,
                TestUsernameA,
                TestPassword);

            using var userBClient = _factory.CreateClient();

            await RegisterUserAsync(
                userBClient,
                TestUsernameB,
                TestPassword);

            await CreateAuthenticatedUserAsync(
                userBClient,
                TestUsernameB,
                TestPassword);

            var deleteResponse = await userBClient.DeleteAsync(
                $"/api/{ApiVersion}/Games/1");

            Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);

            var getReponse = await userAClient.GetAsync(
                $"/api/{ApiVersion}/Games/1");

            Assert.Equal(HttpStatusCode.OK, getReponse.StatusCode);
        }

        [Fact]
        public async Task User_CanOnlySeeTheirOwnGames()
        {
            await _factory.ResetDatabaseAsync();

            using var userAClient = _factory.CreateClient();

            await CreateAuthenticatedUserAsync(
                userAClient,
                TestUsernameA,
                TestPassword);

            using var userBClient = _factory.CreateClient();

            await RegisterUserAsync(
                userBClient,
                TestUsernameB,
                TestPassword);

            await CreateAuthenticatedUserAsync(
                userBClient,
                TestUsernameB,
                TestPassword);

            var getAResponse = await userAClient.GetAsync(
                $"/api/{ApiVersion}/Games");
            var getBResponse = await userBClient.GetAsync(
                $"/api/{ApiVersion}/Games");

            Assert.Equal(HttpStatusCode.OK, getAResponse.StatusCode);
            Assert.Equal(HttpStatusCode.OK, getBResponse.StatusCode);

            var userAJson = await getAResponse.Content.ReadFromJsonAsync<JsonDocument>();
            var userBJson = await getBResponse.Content.ReadFromJsonAsync<JsonDocument>();

            Assert.NotNull(userAJson);
            Assert.NotNull(userBJson);

            var userAGamesCount = userAJson.RootElement.GetProperty("items").GetArrayLength();
            var userBGamesCount = userBJson.RootElement.GetProperty("items").GetArrayLength();

            Assert.Equal(2, userAGamesCount);
            Assert.Equal(0, userBGamesCount);
        }

        [Fact]
        public async Task DifferentUsers_CanHaveGamesWithSameTitle()
        {
            await _factory.ResetDatabaseAsync();

            using var clientA = _factory.CreateClient();
            using var clientB = _factory.CreateClient();

            await CreateAuthenticatedUserAsync(
                clientA, 
                TestUsernameA, 
                TestPassword);

            await RegisterUserAsync(
                clientB,
                TestUsernameB,
                TestPassword);

            await CreateAuthenticatedUserAsync(
                clientB, 
                TestUsernameB, 
                TestPassword);

            var request = new CreateGameDto
            {
                Title = "Test Game",
                GenreIds = new List<int> { 1 },
                PlatformIds = new List<int> { 1 },
                Status = GameStatus.Backlog,
            };

            var responseA = await clientA.PostAsJsonAsync(
                $"/api/{ApiVersion}/Games",
                request);

            var responseB = await clientB.PostAsJsonAsync(
                $"/api/{ApiVersion}/Games",
                request);

            Assert.Equal(HttpStatusCode.Created, responseA.StatusCode);
            Assert.Equal(HttpStatusCode.Created, responseB.StatusCode);

            responseA = await clientA.PostAsJsonAsync(
                $"/api/{ApiVersion}/Games",
                request);

            responseB = await clientB.PostAsJsonAsync(
                $"/api/{ApiVersion}/Games",
                request);

            Assert.Equal(HttpStatusCode.Conflict, responseA.StatusCode);
            Assert.Equal(HttpStatusCode.Conflict, responseB.StatusCode);
        }
    }
}
