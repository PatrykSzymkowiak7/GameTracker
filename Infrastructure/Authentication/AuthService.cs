using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GameTracker.Application.DTOs;
using GameTracker.Application.Exceptions;
using GameTracker.Application.Interfaces;
using GameTracker.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GameTracker.Infrastructure.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtOptions _jwtOptions;

        public AuthService(UserManager<ApplicationUser> userManager,
            IOptions<JwtOptions> jwtOptions)
        {
            _userManager = userManager;
            _jwtOptions = jwtOptions.Value;
        }

        public async Task RegisterAsync(RegisterDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Username,
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(
                user,
                dto.Password);

            if(!result.Succeeded)
            {
                throw new RegistrationException(
                    result.Errors.Select(error => error.Description));
            }
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.Username);

            if (user is null)
            {
                throw new InvalidCredentialsException();
            }

            var passwordValid = await _userManager.CheckPasswordAsync(
                user,
                dto.Password);

            if (!passwordValid)
            {
                throw new InvalidCredentialsException();
            }

            return CreateToken(user);
        }

        private AuthResponseDto CreateToken(ApplicationUser user)
        {
            var expiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.Name, user.UserName!),
                new(ClaimTypes.Email, user.Email!),
                new(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

            return new AuthResponseDto
            {
                AccessToken = new JwtSecurityTokenHandler()
                    .WriteToken(token),

                ExpiresAtUtc = expiresAt
            };
        }
    }
}
