using WebApplication1.Enums;

namespace WebApplication1.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int Age { get; set; }
        public Gender Gender { get; set; }

        public ICollection<Role> Roles { get; set; }
        public ICollection<UserRole> UserRole { get; set; }
    }
}
