using GameTracker.Application.Interfaces;
using GameTracker.Domain.Entities;
using GameTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Infrastructure.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly GameDbContext _context;

        public GameRepository(GameDbContext context)
        {
            _context = context;
        }

        public async Task<Game> CreateAsync(Game game)
        {
            _context.Add(game);
            await _context.SaveChangesAsync();

            return game;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var game = await _context.Games.FindAsync(id);

            if (game is null)
                return false;

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<Game>> GetAllAsync()
        {
            return await _context.Games.ToListAsync();
        }

        public async Task<Game?> GetByIdAsync(int id)
        {
            return await _context.Games.FindAsync(id);
        }

        public async Task UpdateAsync(Game game)
        {
            _context.Games.Update(game);
            await _context.SaveChangesAsync();
        }
    }
}
