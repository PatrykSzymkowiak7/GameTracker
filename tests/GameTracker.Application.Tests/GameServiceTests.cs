using AutoMapper;
using GameTracker.Application.DTOs;
using GameTracker.Application.Exceptions;
using GameTracker.Application.Interfaces;
using GameTracker.Application.Services;
using GameTracker.Domain.Entities;
using Moq;

namespace GameTracker.Application.Tests
{
    public class GameServiceTests
    {
        private Mock<IGameRepository> _repositoryMock;
        private Mock<ICurrentUserService> _currentUserServiceMock;
        private Mock<IMapper> _mapperMock;
        private GameService _gameService;

        public GameServiceTests()
        {
            _repositoryMock = new Mock<IGameRepository>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _mapperMock = new Mock<IMapper>();

            _gameService = new GameService(
                _repositoryMock.Object, 
                _mapperMock.Object, 
                _currentUserServiceMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_WhenGameExistis_ReturnsGameDto()
        {
            // Arrange
            var game = new Game(
                title: "Elden Ring",
                status: GameTracker.Domain.Enums.GameStatus.Playing,
                rating: 10,
                hoursPlayed: 100,
                userId: 1,
                genres: new List<Genre>
                {
                    new Genre (name: "RPG")
                },
                platforms: new List<GamePlatform>
                {
                    new GamePlatform(name: "PC")
                },
                developer: new Developer(name: "Test")
                );

            _repositoryMock.Setup(r => r.GetByIdAsync(1, 1))
                .ReturnsAsync(game);

            var expectedDto = new GameDto
            {
                Id = 1,
                Title = "Elden Ring",
                Status = GameTracker.Domain.Enums.GameStatus.Playing,
                Rating = 10,
                HoursPlayed = 100,
                Genres = new List<GenreDto>
                {
                    new GenreDto { Id = 1, Name = "RPG" }
                },
                Platforms = new List<GamePlatformDto>
                {
                    new GamePlatformDto { Id = 1, Name = "PC" }
                }
            };

            _mapperMock.Setup(m => m.Map<GameDto>(game))
                .Returns(expectedDto);

            _currentUserServiceMock.Setup(m => m.UserId)
                .Returns(1);

            // Act
            var result = await _gameService.GetByIdAsync(1);

            // Assert
            Assert.Equal(expectedDto, result);
        }

        [Fact]
        public async Task GetByIdAsync_WhenGameDoesNotExist_ThrowGameNotFoundException()
        {
            _repositoryMock.Setup(r => r.GetByIdAsync(999, 1))
                .ReturnsAsync((Game?)null);

            _currentUserServiceMock.Setup(m => m.UserId)
                .Returns(1);

            await Assert.ThrowsAsync<GameNotFoundException>(
                () => _gameService.GetByIdAsync(999));
        }

        [Fact]
        public async Task GetAllAsync_ReturnsSomeRecords()
        {
            // Arrange
            var developer = new Developer("From Software");
            var genres = new List<Genre>() { 
                new Genre("RPG") 
            };
            var platforms = new List<GamePlatform>() { 
                new GamePlatform("PC") 
            };

            var game1 = new Game(
                title: "Elden Ring",
                genres: genres,
                platforms: platforms,
                status: GameTracker.Domain.Enums.GameStatus.Playing,
                rating: 10,
                hoursPlayed: 100,
                developer: developer,
                userId: 1);

            var game2 = new Game(
                title: "The First Berserker: Khazan",
                genres: genres,
                platforms: platforms,
                status: GameTracker.Domain.Enums.GameStatus.Playing,
                rating: 9,
                hoursPlayed: 50,
                developer: new Developer("From Software"),
                userId: 1);

            IEnumerable<Game> items = new List<Game>() { game1, game2 };

            var genreDtos = new List<GenreDto>() { 
                new GenreDto() { Id = 1, Name = "RPG" } 
            };
            var platformDtos = new List<GamePlatformDto>() { 
                new GamePlatformDto { Id = 1, Name = "PC" } 
            };

            GameDto expectedDto1 = new GameDto
            {
                Id = 1,
                Title = "Elden Ring",
                Status = GameTracker.Domain.Enums.GameStatus.Playing,
                Rating = 10,
                HoursPlayed = 100,
                Genres = genreDtos,
                Platforms = platformDtos
            };

            GameDto expectedDto2 = new GameDto
            {
                Id = 2,
                Title = "The First Berserker: Khazan",
                Status = GameTracker.Domain.Enums.GameStatus.Playing,
                Rating = 9,
                HoursPlayed = 50,
                Genres = genreDtos,
                Platforms = platformDtos
            };

            var expectedItems = new List<GameDto> { expectedDto1, expectedDto2 };

            PagedResultDto<GameDto> expectedResult = new PagedResultDto<GameDto>()
            {
                Items = expectedItems,
                Page = 1,
                PageSize = 10,
                TotalCount = 2
            };

            GameQueryDto gameQuery = new GameQueryDto()
            {
                Status = GameTracker.Domain.Enums.GameStatus.Playing,
                GenreId = 1,
                Page = 1,
                PlatformId = 1,
                Descending = false,
                SortBy = string.Empty
            };

            _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<GameQueryDto>(), 1))
                .ReturnsAsync((items, items.Count()));

            _mapperMock.Setup(m => m.Map<IEnumerable<GameDto>>(items))
                .Returns(expectedItems);

            _currentUserServiceMock.Setup(m => m.UserId)
                .Returns(1);

            // Act
            var result = await _gameService.GetAllAsync(gameQuery);

            // Assert
            Assert.Equal(2, result.Items.Count());

            var resultItems = result.Items.ToList();

            Assert.Equal(expectedDto1.Title, resultItems[0].Title);
            Assert.Equal(expectedDto2.Title, resultItems[1].Title);
        }

        [Fact]
        public async Task CreateAsync_CreatesGameAndReturnsDto()
        {
            var createGameDto = new CreateGameDto()
            {
                Title = "Elden Ring",
                DeveloperId = 1,
                GenreIds = new List<int> { 1 },
                PlatformIds = new List<int> { 1 },
                Status = Domain.Enums.GameStatus.Completed,
                Rating = 10,
                HoursPlayed = 100
            };

            _currentUserServiceMock.Setup(x => x.UserId)
                .Returns(1);

            _repositoryMock.Setup(x => x.GetDeveloperByIdAsync(1))
                .ReturnsAsync(new Developer("From Software"));

            _repositoryMock.Setup(x => x.GetGenresByIdsAsync(It.IsAny<IEnumerable<int>>()))
                .ReturnsAsync(new List<Genre>() { new Genre("RPG") });

            _repositoryMock.Setup(x => x.GetGamePlatformsByIdsAsync(It.IsAny<IEnumerable<int>>()))
                .ReturnsAsync(new List<GamePlatform>() { new GamePlatform("PC") });

            _repositoryMock.Setup(x => x.CreateAsync(It.IsAny<Game>()))
                .ReturnsAsync((Game game) => game);

            var gameDto = await _gameService.CreateAsync(createGameDto);

            _repositoryMock.Verify(x => x.CreateAsync(
                    It.Is<Game>(g =>
                    g.Title == createGameDto.Title &&
                    g.Developer!.Name == "From Software" &&
                    g.Genres.Count == 1 &&
                    g.Platforms.Count == 1 &&
                    g.UserId == _currentUserServiceMock.Object.UserId)),
                Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WhenDeveloperNotExists_ReturnsDeveloperNotFoundException()
        {
            _currentUserServiceMock.Setup(x => x.UserId)
                .Returns(1);

            _repositoryMock.Setup(x => x.GetDeveloperByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Developer?)null);

            _repositoryMock.Setup(x => x.GetGenresByIdsAsync(It.IsAny<IEnumerable<int>>()))
    .           ReturnsAsync(new List<Genre>() { new Genre("RPG") });

            _repositoryMock.Setup(x => x.GetGamePlatformsByIdsAsync(It.IsAny<IEnumerable<int>>()))
                .ReturnsAsync(new List<GamePlatform>() { new GamePlatform("PC") });

            var createGameDto = new CreateGameDto()
            {
                Title = "Elden Ring",
                DeveloperId = 1,
                GenreIds = new List<int> { 1 },
                PlatformIds = new List<int> { 1 },
                Status = Domain.Enums.GameStatus.Completed,
                Rating = 10,
                HoursPlayed = 100
            };

            await Assert.ThrowsAsync<DeveloperNotFoundException>(() => _gameService.CreateAsync(createGameDto));
        }

        [Fact]
        public async Task CreateAsync_WhenDuplicateGenres_ReturnsGameValidationException()
        {
            _currentUserServiceMock.Setup(x => x.UserId)
                .Returns(1);

            _repositoryMock.Setup(x => x.GetDeveloperByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new Developer("From Software"));

            _repositoryMock.Setup(x => x.GetGenresByIdsAsync(It.IsAny<IEnumerable<int>>()))
                .ReturnsAsync(new List<Genre>() { new Genre("RPG"), new Genre("Action") });

            _repositoryMock.Setup(x => x.GetGamePlatformsByIdsAsync(It.IsAny<IEnumerable<int>>()))
                .ReturnsAsync(new List<GamePlatform>() { new GamePlatform("PC") });

            var createGameDto = new CreateGameDto()
            {
                Title = "Elden Ring",
                DeveloperId = 1,
                GenreIds = new List<int> { 1, 2, 1 },
                PlatformIds = new List<int> { 1 },
                Status = Domain.Enums.GameStatus.Completed,
                Rating = 10,
                HoursPlayed = 100
            };

            await Assert.ThrowsAsync<GameValidationException>(() => _gameService.CreateAsync(createGameDto));
        }

        [Fact]
        public async Task CreateAsync_WhenDuplicatePlatforms_ReturnsGameValidationException()
        {
            _currentUserServiceMock.Setup(x => x.UserId)
                .Returns(1);

            _repositoryMock.Setup(x => x.GetDeveloperByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new Developer("From Software"));

            _repositoryMock.Setup(x => x.GetGenresByIdsAsync(It.IsAny<IEnumerable<int>>()))
                .ReturnsAsync(new List<Genre>() { new Genre("RPG") });

            _repositoryMock.Setup(x => x.GetGamePlatformsByIdsAsync(It.IsAny<IEnumerable<int>>()))
                .ReturnsAsync(new List<GamePlatform>() { new GamePlatform("PC"), new GamePlatform("PS5") });

            var createGameDto = new CreateGameDto()
            {
                Title = "Elden Ring",
                DeveloperId = 1,
                GenreIds = new List<int> { 1 },
                PlatformIds = new List<int> { 1, 2, 1 },
                Status = Domain.Enums.GameStatus.Completed,
                Rating = 10,
                HoursPlayed = 100
            };

            await Assert.ThrowsAsync<GameValidationException>(() => _gameService.CreateAsync(createGameDto));
        }

        [Fact]
        public async Task CreateAsync_WhenGenreNotFound_ReturnsGameValidationException()
        {
            _currentUserServiceMock.Setup(x => x.UserId)
                .Returns(1);

            _repositoryMock.Setup(x => x.GetDeveloperByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new Developer("From Software"));

            _repositoryMock.Setup(x => x.GetGenresByIdsAsync(It.IsAny<IEnumerable<int>>()))
                .ReturnsAsync(new List<Genre>());

            _repositoryMock.Setup(x => x.GetGamePlatformsByIdsAsync(It.IsAny<IEnumerable<int>>()))
                .ReturnsAsync(new List<GamePlatform>() { new GamePlatform("PC") });

            var createGameDto = new CreateGameDto()
            {
                Title = "Elden Ring",
                DeveloperId = 1,
                GenreIds = new List<int> { 1 },
                PlatformIds = new List<int> { 1 },
                Status = Domain.Enums.GameStatus.Completed,
                Rating = 10,
                HoursPlayed = 100
            };

            await Assert.ThrowsAsync<GameValidationException>(() => _gameService.CreateAsync(createGameDto));
        }

        [Fact]
        public async Task CreateAsync_WhenPlatformNotFound_ReturnsGameValidationException()
        {
            _currentUserServiceMock.Setup(x => x.UserId)
                .Returns(1);

            _repositoryMock.Setup(x => x.GetDeveloperByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new Developer("From Software"));

            _repositoryMock.Setup(x => x.GetGenresByIdsAsync(It.IsAny<IEnumerable<int>>()))
                .ReturnsAsync(new List<Genre>() { new Genre("RPG") });

            _repositoryMock.Setup(x => x.GetGamePlatformsByIdsAsync(It.IsAny<IEnumerable<int>>()))
                .ReturnsAsync(new List<GamePlatform>());

            var createGameDto = new CreateGameDto()
            {
                Title = "Elden Ring",
                DeveloperId = 1,
                GenreIds = new List<int> { 1 },
                PlatformIds = new List<int> { 1 },
                Status = Domain.Enums.GameStatus.Completed,
                Rating = 10,
                HoursPlayed = 100
            };

            await Assert.ThrowsAsync<GameValidationException>(() => _gameService.CreateAsync(createGameDto));
        }
    }
}