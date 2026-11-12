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
        [Fact]
        public async Task GetByIdAsync_WhenGameExistis_ReturnsGameDto()
        {
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

            var repositoryMock = new Mock<IGameRepository>();

            repositoryMock
                .Setup(r => r.GetByIdAsync(1, 1))
                .ReturnsAsync(game);

            var mapperMock = new Mock<IMapper>();

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

            mapperMock
                .Setup(m => m.Map<GameDto>(game))
                .Returns(expectedDto);

            var currentUserServiceMock = new Mock<ICurrentUserService>();
            currentUserServiceMock.Setup(m => m.UserId)
                .Returns(1);

            var service = new GameService(
                repositoryMock.Object,
                mapperMock.Object, 
                currentUserServiceMock.Object);

            var result = await service.GetByIdAsync(1);

            Assert.Equal(expectedDto, result);
        }

        [Fact]
        public async Task GetByIdAsync_WhenGameDoesNotExist_ThrowGameNotFoundException()
        {
            var repositoryMock = new Mock<IGameRepository>();

            repositoryMock
                .Setup(r => r.GetByIdAsync(999, 1))
                .ReturnsAsync((Game?)null);

            var mapperMock = new Mock<IMapper>();

            var currentUserServiceMock = new Mock<ICurrentUserService>();
            currentUserServiceMock.Setup(m => m.UserId)
                .Returns(1);

            var service = new GameService(
                repositoryMock.Object,
                mapperMock.Object,
                currentUserServiceMock.Object);

            await Assert.ThrowsAsync<GameNotFoundException>(
                () => service.GetByIdAsync(999));
        }

        [Fact]
        public async Task GetAllAsync_ReturnsSomeRecords()
        {
            var repositoryMock = new Mock<IGameRepository>();

            var game1 = new Game(
                title: "Elden Ring",
                genres: new List<Genre>
                {
                    new Genre(name: "RPG")
                },
                platforms: new List<GamePlatform>
                {
                    new GamePlatform(name: "PC")
                },
                status: GameTracker.Domain.Enums.GameStatus.Playing,
                rating: 10,
                hoursPlayed: 100,
                developer: new Developer("From Software"),
                userId: 1);

            var game2 = new Game(
                title: "The First Berserker: Khazan",
                genres: new List<Genre>
                {
                    new Genre(name: "RPG" )
                },
                platforms: new List<GamePlatform>
                {
                    new GamePlatform(name: "PC")
                },
                status: GameTracker.Domain.Enums.GameStatus.Playing,
                rating: 9,
                hoursPlayed: 50,
                developer: new Developer("From Software"),
                userId: 1);

            IEnumerable<Game> items = new List<Game>() { game1, game2 };

            GameDto expectedDto1 = new GameDto
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

            GameDto expectedDto2 = new GameDto
            {
                Id = 2,
                Title = "The First Berserker: Khazan",
                Status = GameTracker.Domain.Enums.GameStatus.Playing,
                Rating = 9,
                HoursPlayed = 50,
                Genres = new List<GenreDto>
                {
                    new GenreDto { Id = 1, Name = "RPG" }
                },
                Platforms = new List<GamePlatformDto>
                {
                    new GamePlatformDto { Id = 1, Name = "PC" }
                }

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

            repositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<GameQueryDto>(), 1))
                .ReturnsAsync((items, items.Count()));

            var mapperMock = new Mock<IMapper>();

            mapperMock
                .Setup(m => m.Map<IEnumerable<GameDto>>(items))
                .Returns(expectedItems);

            var currentUserServiceMock = new Mock<ICurrentUserService>();
            currentUserServiceMock.Setup(m => m.UserId)
                .Returns(1);

            var service = new GameService(
                repositoryMock.Object,
                mapperMock.Object,
                currentUserServiceMock.Object);

            var result = await service.GetAllAsync(gameQuery);

            Assert.Equal(2, result.Items.Count());

            var resultItems = result.Items.ToList();

            Assert.Equal(expectedDto1.Title, resultItems[0].Title);
            Assert.Equal(expectedDto2.Title, resultItems[1].Title);
        }
    }
}