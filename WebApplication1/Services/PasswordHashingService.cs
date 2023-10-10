using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Services
{
    public class PasswordHashingService : IPasswordHashingService
    {
        private readonly IPasswordHasher<object> _passwordHasher;

        public PasswordHashingService()
        {
            _passwordHasher = new PasswordHasher<object>();
        }

        public string HashPassword(string password)
        {
            return _passwordHasher.HashPassword(null, password);
        }

        public bool CheckPassword(string providedPassword, string storedHash)
        {
            var result = _passwordHasher.VerifyHashedPassword(null, storedHash, providedPassword);
            return result == PasswordVerificationResult.Success;
        }
    }
}
