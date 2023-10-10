using WebApplication1.Models;
using WebApplication1.Queries;

namespace WebApplication1.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync(UserQuery query, CancellationToken cancellationToken);
        Task<User> GetByUsernameAsync(string username, CancellationToken cancellationToken);
    }
}
