using GameTracker.Application.DTOs;
using GameTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Application.Interfaces
{
    public interface IGameRepository
    {
        Task<(IEnumerable<Game> Items, int TotalCount)> GetAllAsync(GameQueryDto query);
        Task<Game?> GetByIdAsync(int id);
        Task<Game> CreateAsync(Game game);
        Task UpdateAsync(Game game);
        Task<bool> DeleteAsync(int id);
        Task<Game?> GetByTitleAsync(string title);
        Task<List<Genre>> GetGenresByIdsAsync(IEnumerable<int> ids);
        Task<List<GamePlatform>> GetGamePlatformsByIdsAsync(IEnumerable<int> ids);
        Task<Developer?> GetDeveloperByIdAsync(int id);
        Task<IEnumerable<Genre>> GetAllGenresAsync();
        Task<IEnumerable<GamePlatform>> GetAllPlatformsAsync();
        Task<IEnumerable<Developer>> GetAllDevelopersAsync();
    }
}
