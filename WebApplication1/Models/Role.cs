using WebApplication1.Enums;

namespace WebApplication1.Models
{
    public class Role
    {
        public Guid Id { get; set; }
        public RoleName Name { get; set; } 

        public ICollection<User> Users { get; set; }
        public ICollection<UserRole> UserRole { get; set; }
    }
}
