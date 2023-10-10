using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.Persistance;

namespace WebApplication1.Services
{
    public class RefreshTokenService
    {
        private readonly DbContextUsers _context;

        public RefreshTokenService(DbContextUsers context)
        {
            _context = context;
        }

        public async Task<string> CreateRefreshTokenAsync(User user)
        {
            var token = $"{Guid.NewGuid()}{Guid.NewGuid()}".Replace("-", "");

            var existentToken = await _context.RefreshTokens.SingleOrDefaultAsync(t => t.UserId == user.Id);
            if (existentToken != null)
            {
                _context.Remove(existentToken);
            }

            var newToken = new RefreshToken()
            {
                Token = token,
                UserId = user.Id
            };
            
            await _context.AddAsync(newToken);
            await _context.SaveChangesAsync();

            return token;
        }
    }
}
