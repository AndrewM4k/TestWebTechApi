using WebApplication1.Enums;
using WebApplication1.Models;

namespace WebApplication1.Dtos
{
    public class GetUserDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public Gender Gender { get; set; }
        public IEnumerable<RoleName> Roles { get; set; }
    }
}
