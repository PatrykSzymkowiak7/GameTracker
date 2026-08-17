using GameTracker.Application.DTOs;
using GameTracker.Application.Interfaces;
using GameTracker.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GameTracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GamesController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await _gameService.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<PagedResultDto<GameDto>>> GetAll(
            [FromQuery] GameQueryDto query)
        {
            var games = await _gameService.GetAllAsync(query);

            return Ok(games);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GameDto?>> GetByIdAsync(int id)
        {
            var game = await _gameService.GetByIdAsync(id);

            return Ok(game);
        }

        [HttpPost]
        public async Task<ActionResult<GameDto>> Create(CreateGameDto dto)
        {
            var game = await _gameService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(Create),
                new { id = game.Id },
                game
                );
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GameDto>> Update(int id, UpdateGameDto dto)
        {
            var game = await _gameService.UpdateAsync(id, dto);

            return game != null ? Ok(game) : NotFound();
        }
    }
}
