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

            if(!(existingGame is null))
                throw new GameConflictException($"Game '{dto.Title}' already exists.");

            var game = _mapper.Map<Game>(dto);

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
