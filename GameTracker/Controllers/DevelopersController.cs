using Asp.Versioning;
using GameTracker.Application.DTOs;
using GameTracker.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GameTracker.Api.Controllers
{
    [ApiVersion(1.0)]
    [ApiController]
    [Route("api/v{version:apiVersion}/developers")]
    public class DevelopersController : ControllerBase
    {
        private readonly IGameService _gameService;

        public DevelopersController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeveloperDto>>> GetAll()
        {
            var developers = await _gameService.GetDevelopersAsync();

            return Ok(developers);
        }
    }
}
