using GameTracker.Application.Interfaces;
using System.Security.Claims;

namespace GameTracker.Api.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int UserId
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;

                var userIdClaim = user?.FindFirstValue(ClaimTypes.NameIdentifier);

                if(!int.TryParse(userIdClaim, out var userId))
                {
                    throw new UnauthorizedAccessException();
                }

                return userId;
            }
        }
    }
}
