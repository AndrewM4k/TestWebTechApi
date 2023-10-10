using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication1.Models;
using WebApplication1.Persistance;

namespace WebApplication1.Services
{
    public class TokenService
    {
        private readonly DbContextUsers _context;
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration configuration, DbContextUsers context)
        {
            _key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["TokenSecret"]));
            _context = context;
        }

        public async Task<string> CreateTokenAsync(User user)
        {
            var claims = new List<Claim>()
            {
                new (JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new (JwtRegisteredClaimNames.UniqueName, user.Name),
            }; 
            var roles = _context.Roles
                .Include(r => r.Users)
                .Where(u => u.Users.Contains(user));

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role.Name.ToString())));

            var creds = new SigningCredentials(_key,
                SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(2),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
