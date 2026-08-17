using GameTracker.Application.DTOs;
using GameTracker.Domain.Entities;

namespace GameTracker.Application.Interfaces
{
    public interface IGameService
    {
        Task<PagedResultDto<GameDto>> GetAllAsync(GameQueryDto query);
        Task<GameDto> GetByIdAsync(int id);
        Task<GameDto> CreateAsync(CreateGameDto dto);
        Task<GameDto?> UpdateAsync(int id, UpdateGameDto dto);
        Task<bool> DeleteAsync(int id);

    }
}
