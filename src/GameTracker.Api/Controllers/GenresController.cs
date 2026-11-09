using Asp.Versioning;
using GameTracker.Application.DTOs;
using GameTracker.Application.Interfaces;
using GameTracker.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GameTracker.Api.Controllers
{
    [ApiVersion(1.0)]
    [ApiController]
    [Route("api/v{version:apiVersion}/genres")]
    public class GenresController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GenresController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GenreDto>>> GetAll()
        {
            var genres = await _gameService.GetGenresAsync();

            return Ok(genres);
        }
    }
}
