using WebApplication1.Enums;
using WebApplication1.Models;

namespace WebApplication1.Dtos
{
    public class UpdateUserDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Age { get; set; }
        public Gender Gender { get; set; }
    }
}
