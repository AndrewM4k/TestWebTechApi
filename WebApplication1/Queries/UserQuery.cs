using WebApplication1.Enums;

namespace WebApplication1.Queries
{
    public class UserQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;

        public Gender? Gender { get; set; }
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public IEnumerable<RoleName> Roles { get; set; }

        public string SortBy { get; set; } = "Name";
        public bool SortAscending { get; set; } = true;
    }
}
