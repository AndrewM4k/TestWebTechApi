using WebApplication1.Enums;
using WebApplication1.Models;
using WebApplication1.Queries;

namespace WebApplication1.Repositories
{
    public interface IUserRepository
    {
        Task AddRoleToUserAsync(Guid userId, RoleName role, CancellationToken cancellationToken);
        Task AddUserAsync(User user, CancellationToken cancellationToken);
        Task<IEnumerable<User>> GetAllAsync(UserQuery query, CancellationToken cancellationToken);
        Task<User> GetByUsernameAsync(string username, CancellationToken cancellationToken);
        Task<User> GetUserWithRolesById(Guid id, CancellationToken cancellationToken);
        Task RemoveAsync(Guid id, CancellationToken cancellationToken);
        Task UpdateUserAsync(Guid id, User newUser, CancellationToken cancellationToken);
    }
}
