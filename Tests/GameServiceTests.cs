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
            var game = new Game
            {
                Id = 1,
                Title = "Elden Ring",
                Genre = "RPG",
                Platform = GameTracker.Domain.Enums.Platform.PC,
                Status = GameTracker.Domain.Enums.GameStatus.Playing,
                Rating = 10,
                HoursPlayed = 100
            };

            var repositoryMock = new Mock<IGameRepository>();

            repositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(game);

            var mapperMock = new Mock<IMapper>();

            var expectedDto = new GameDto
            {
                Id = 1,
                Title = "Elden Ring",
                Genre = "RPG",
                Platform = GameTracker.Domain.Enums.Platform.PC,
                Status = GameTracker.Domain.Enums.GameStatus.Playing,
                Rating = 10,
                HoursPlayed = 100
            };

            mapperMock
                .Setup(m => m.Map<GameDto>(game))
                .Returns(expectedDto);

            var service = new GameService(
                repositoryMock.Object,
                mapperMock.Object);

            var result = await service.GetByIdAsync(1);

            Assert.Equal(expectedDto, result);
        }

        [Fact]
        public async Task GetByIdAsync_WhenGameDoesNotExist_ThrowGameNotFoundException()
        {
            var repositoryMock = new Mock<IGameRepository>();

            repositoryMock
                .Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Game?)null);

            var mapperMock = new Mock<IMapper>();

            var service = new GameService(
                repositoryMock.Object,
                mapperMock.Object);

            await Assert.ThrowsAsync<GameNotFoundException>(
                () => service.GetByIdAsync(999));
        }

        [Fact]
        public async Task GetAllAsync_ReturnsSomeRecords()
        {
            var repositoryMock = new Mock<IGameRepository>();

            var game1 = new Game
            {
                Id = 1,
                Title = "Elden Ring",
                Genre = "RPG",
                Platform = GameTracker.Domain.Enums.Platform.PC,
                Status = GameTracker.Domain.Enums.GameStatus.Playing,
                Rating = 10,
                HoursPlayed = 100
            };

            var game2 = new Game
            {
                Id = 2,
                Title = "The First Berserker: Khazan",
                Genre = "RPG",
                Platform = GameTracker.Domain.Enums.Platform.PC,
                Status = GameTracker.Domain.Enums.GameStatus.Playing,
                Rating = 9,
                HoursPlayed = 50
            };

            IEnumerable<Game> items = new List<Game>() { game1, game2 };

            GameDto expectedDto1 = new GameDto
            {
                Id = 1,
                Title = "Elden Ring",
                Genre = "RPG",
                Platform = GameTracker.Domain.Enums.Platform.PC,
                Status = GameTracker.Domain.Enums.GameStatus.Playing,
                Rating = 10,
                HoursPlayed = 100
            };

            GameDto expectedDto2 = new GameDto
            {
                Id = 2,
                Title = "The First Berserker: Khazan",
                Genre = "RPG",
                Platform = GameTracker.Domain.Enums.Platform.PC,
                Status = GameTracker.Domain.Enums.GameStatus.Playing,
                Rating = 9,
                HoursPlayed = 50
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
                Genre = "RPG",
                Page = 1,
                Platform = GameTracker.Domain.Enums.Platform.PC,
                Descending = false,
                SortBy = string.Empty
            };

            repositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<GameQueryDto>()))
                .ReturnsAsync((items, items.Count()));

            var mapperMock = new Mock<IMapper>();

            mapperMock
                .Setup(m => m.Map<IEnumerable<GameDto>>(items))
                .Returns(expectedItems);

            var service = new GameService(
                repositoryMock.Object,
                mapperMock.Object);

            var result = await service.GetAllAsync(gameQuery);

            Assert.Equal(2, result.Items.Count());

            var resultItems = result.Items.ToList();

            Assert.Equal(expectedDto1.Title, resultItems[0].Title);
            Assert.Equal(expectedDto2.Title, resultItems[1].Title);
        }
    }
}