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
    }
}
