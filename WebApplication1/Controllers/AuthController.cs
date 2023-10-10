using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsCompetition.Dtos;
using WebApplication1.Enums;
using WebApplication1.Models;
using WebApplication1.Persistance;
using WebApplication1.Repositories;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly RefreshTokenService _refreshTokenService;
        private readonly DbContextUsers _context;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHashingService _passwordHasing;

        public AuthController(
            TokenService tokenService,
            RefreshTokenService refreshTokenService,
            DbContextUsers context,
            IUserRepository userRepository,
            IPasswordHashingService passwordHasing)
        {
            _tokenService = tokenService;
            _refreshTokenService = refreshTokenService;
            _context = context;
            _userRepository = userRepository;
            _passwordHasing = passwordHasing;
        }

        [HttpPost("singIn")]
        public async Task<ActionResult<SignInResultDto>> SignInAsync(SingInDto dto, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByUsernameAsync(dto.Username, cancellationToken);
            if (user == null)
            {
                return Unauthorized();
            }

            if (!_passwordHasing.CheckPassword(dto.Password, user.PasswordHash))
            {
                return Unauthorized();
            }

            var token = await _tokenService.CreateTokenAsync(user, cancellationToken);
            var refreshToken = await _refreshTokenService.CreateRefreshTokenAsync(user);

            var result = new SignInResultDto
            {
                AccessToken = token,
                RefreshToken = refreshToken
            };

            return result;
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<RefreshTokenResultDto>> RefreshTokenAsync(RefreshTokenDto dto, CancellationToken cancellationToken)
        {
            var token = await _context.RefreshTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Token == dto.RefreshToken);

            if (token == null)
            {
                return BadRequest();
            }

            var result = new RefreshTokenResultDto
            {
                AccessToken = await _tokenService.CreateTokenAsync(token.User, cancellationToken),
                RefreshToken = await _refreshTokenService.CreateRefreshTokenAsync(token.User)
            };

            return result;
        }
    }
}
