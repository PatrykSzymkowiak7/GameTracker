using AutoMapper;
using GameTracker.Application.DTOs;
using GameTracker.Application.Exceptions;
using GameTracker.Application.Interfaces;
using GameTracker.Domain.Entities;
using System.Reflection.Metadata.Ecma335;

namespace GameTracker.Application.Services
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _repository;
        private readonly IMapper _mapper;

        public GameService(IGameRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<GameDto> CreateAsync(CreateGameDto dto)
        {
            var existingGame = await _repository.GetByTitleAsync(dto.Title);

            if(existingGame is not null)
                throw new GameConflictException($"Game '{dto.Title}' already exists.");

            var game = _mapper.Map<Game>(dto);

            if(dto.DeveloperId.HasValue)
            {
                var developer = await _repository
                    .GetDeveloperByIdAsync(dto.DeveloperId.Value);

                if(developer is null)
                {
                    throw new DeveloperNotFoundException(dto.DeveloperId.Value);
                }

                game.Developer = developer;
            }

            var genres = await _repository
                .GetGenresByIdsAsync(dto.GenreIds);

            if(genres.Count != dto.GenreIds.Count)
            {
                throw new GameValidationException(
                    "One or more specified genres do not exist.");
            }

            var platforms = await _repository
                .GetGamePlatformsByIdsAsync(dto.PlatformIds);

            if(platforms.Count != dto.PlatformIds.Count)
            {
                throw new GameValidationException(
                    "One or more specified platforms do not exist.");
            }

            game.Platforms = platforms;
            game.Genres = genres;

            var createdGame = await _repository.CreateAsync(game);

            return _mapper.Map<GameDto>(createdGame);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<PagedResultDto<GameDto>> GetAllAsync(GameQueryDto query)
        {
            var result = await _repository.GetAllAsync(query);

            var gameDtos = _mapper.Map<IEnumerable<GameDto>>(result.Items);

            return new PagedResultDto<GameDto>
            {
                Items = gameDtos,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = result.TotalCount
            };
        }

        public async Task<GameDto> GetByIdAsync(int id)
        {
            var game = await _repository.GetByIdAsync(id);

            if (game is null)
                throw new GameNotFoundException(id);

            return _mapper.Map<GameDto>(game);
        }

        public async Task<IEnumerable<DeveloperDto>> GetDevelopersAsync()
        {
            var developers = await _repository.GetAllDevelopersAsync();

            return _mapper.Map<IEnumerable<DeveloperDto>>(developers);
        }

        public async Task<IEnumerable<GenreDto>> GetGenresAsync()
        {
            var genres = await _repository.GetAllGenresAsync();

            return _mapper.Map<IEnumerable<GenreDto>>(genres);
        }

        public async Task<IEnumerable<GamePlatformDto>> GetPlatformsAsync()
        {
            var platforms = await _repository.GetAllPlatformsAsync();

            return _mapper.Map<IEnumerable<GamePlatformDto>>(platforms);
        }

        public async Task<GameDto?> UpdateAsync(int id, UpdateGameDto dto)
        {
            var game = await _repository.GetByIdAsync(id);

            if (game is null)
                return null;

            _mapper.Map(dto, game);

            await _repository.UpdateAsync(game);

            return _mapper.Map<GameDto>(game);
        }
    }
}
