using GameTracker.Application.DTOs;
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

        public async Task<(IEnumerable<Game> Items, int TotalCount)> GetAllAsync(GameQueryDto query)
        {
            IQueryable<Game> games = _context.Games;

            if (query.Status.HasValue)
                games = games.Where(g => g.Status == query.Status.Value);

            if(query.Platform.HasValue)
                games = games.Where(g => g.Platform == query.Platform.Value);

            if(!string.IsNullOrWhiteSpace(query.Genre))
                games = games.Where(g => g.Genre == query.Genre);

            if(!string.IsNullOrWhiteSpace(query.SortBy))
            {
                games = query.SortBy.ToLower() switch
                {
                    "title" => query.Descending
                    ? games.OrderByDescending(g => g.Title)
                    : games.OrderBy(g => g.Title),

                    "rating" => query.Descending
                    ? games.OrderByDescending(g => g.Rating)
                    : games.OrderBy(g => g.Rating),

                    "hoursplayed" => query.Descending
                    ? games.OrderByDescending(g => g.HoursPlayed)
                    : games.OrderBy(g => g.HoursPlayed)
                };
            }

            if (query.Page < 1)
                query.Page = 1;

            if (query.PageSize < 1)
                query.PageSize = 10;

            if (query.PageSize > 100)
                query.PageSize = 100;

            var totalCount = await games.CountAsync();

            var skip = (query.Page - 1) * query.PageSize;
            var items = await games
                .Skip(skip)
                .Take(query.PageSize)
                .ToListAsync();

            return (items, totalCount);
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
