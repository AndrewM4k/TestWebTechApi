using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SportsCompetition.Dtos
{
    public class SingUpDto
    {
        public string Username { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
