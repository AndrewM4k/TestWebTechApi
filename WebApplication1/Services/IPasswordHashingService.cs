namespace WebApplication1.Services
{
    public interface IPasswordHashingService
    {
        bool CheckPassword(string providedPassword, string storedHash);
        string HashPassword(string password);
    }
}