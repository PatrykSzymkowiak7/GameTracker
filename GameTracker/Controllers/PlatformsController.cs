using Asp.Versioning;
using GameTracker.Application.DTOs;
using GameTracker.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GameTracker.Api.Controllers
{
    [ApiVersion(1.0)]
    [ApiController]
    [Route("api/v{version:apiVersion}/platforms")]
    public class PlatformsController : ControllerBase
    {
        private readonly IGameService _gameService;

        public PlatformsController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GamePlatformDto>>> GetAll()
        {
            var platforms = await _gameService.GetPlatformsAsync();

            return Ok(platforms);
        }
    }
}
