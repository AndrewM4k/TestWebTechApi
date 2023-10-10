using Microsoft.EntityFrameworkCore;
using System.Data.SqlTypes;
using System.Linq.Expressions;
using WebApplication1.Enums;
using WebApplication1.Models;
using WebApplication1.Persistance;
using WebApplication1.Queries;

namespace WebApplication1.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DbContextUsers _context;

        private readonly Dictionary<string, Func<IQueryable<User>, object, IQueryable<User>>> _filterMappings = new()
        {
            { nameof(UserQuery.MinAge), (query, value) => query.Where(user => user.Age >= (int)value) },
            { nameof(UserQuery.MaxAge), (query, value) => query.Where(user => user.Age <= (int)value) },
            { nameof(UserQuery.Gender), (query, value) => query.Where(user => user.Gender == (Gender)value) },
            { nameof(UserQuery.Email), (query, value) => query.Where(user => EF.Functions.Like(user.Email.ToLower(), $"%{(string)value}%".ToLower())) },
            { nameof(UserQuery.Username), (query, value) => query.Where(user => EF.Functions.Like(user.Username.ToLower(), $"%{(string)value}%".ToLower())) },
            { nameof(UserQuery.Name), (query, value) => query.Where(user => EF.Functions.Like(user.Name.ToLower(), $"%{(string)value}%".ToLower())) }
        };

        public UserRepository(DbContextUsers context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync(UserQuery userQuery, CancellationToken cancellationToken)
        {
            var query = _context.Users.Include(x => x.Roles).AsQueryable();

            query = ApplyFilters(query, userQuery);
            query = ApplySorting(query, userQuery);

            if (userQuery.Page > 0 && userQuery.PageSize > 0)
            {
                query = query.Skip((userQuery.Page - 1) * userQuery.PageSize).Take(userQuery.PageSize);
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<User> GetByUsernameAsync(string username, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username, cancellationToken);

            return user;
        }

        public async Task AddUserAsync(User user, CancellationToken cancellationToken)
        {
            await _context.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<User> GetUserWithRolesById(Guid id, CancellationToken cancellationToken)
        {
            var user = await _context.Users.Include(x => x.Roles).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            return user;
        }

        public async Task AddRoleToUserAsync(Guid userId, RoleName role, CancellationToken cancellationToken)
        {
            var user = await GetUserWithRolesById(userId, cancellationToken);
            var dbRole = await _context.Roles.FirstOrDefaultAsync(x => x.Name == role);
            user.Roles.Add(dbRole);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(Guid id, User newUser, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (user == null)
            {
                throw new InvalidOperationException($"Cannot find user with id {id}");
            }

            user.Email = newUser.Email;
            user.Age = newUser.Age;
            user.Gender = newUser.Gender;
            user.Name = newUser.Name;
            user.Username = newUser.Username;

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task RemoveAsync(Guid id, CancellationToken cancellationToken)
        {
            var oldUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (oldUser == null)
            {
                return;
            }

            _context.Remove(oldUser);
            await _context.SaveChangesAsync(cancellationToken);
        }

        private IQueryable<User> ApplyFilters(IQueryable<User> query, UserQuery userQuery)
        {
            foreach (var prop in userQuery.GetType().GetProperties())
            {
                var val = prop.GetValue(userQuery);

                if (val != null && _filterMappings.ContainsKey(prop.Name))
                {
                    var expression = _filterMappings[prop.Name];
                    query = expression(query, val);
                }
            }
            return query;
        }

        private IQueryable<User> ApplySorting(IQueryable<User> query, UserQuery userQuery)
        {
            var propertyInfo = typeof(User).GetProperty(userQuery.SortBy);
            if (propertyInfo != null)
            {
                query = userQuery.SortAscending
                    ? query.OrderBy(ToLambda(propertyInfo.Name))
                    : query.OrderByDescending(ToLambda(propertyInfo.Name));
            }
            return query;
        }

        private Expression<Func<User, object>> ToLambda(string propertyName)
        {
            var parameter = Expression.Parameter(typeof(User));
            var property = Expression.Property(parameter, propertyName);
            var propAsObject = Expression.Convert(property, typeof(object));

            return Expression.Lambda<Func<User, object>>(propAsObject, parameter);
        }
    }
}
