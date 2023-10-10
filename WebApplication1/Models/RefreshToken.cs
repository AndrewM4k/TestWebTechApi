using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Models
{
    public class RefreshToken
    {
        public string Token { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
