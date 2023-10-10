using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Globalization;
using WebApplication1.Controllers;
using WebApplication1.Dtos;
using WebApplication1.Enums;
using WebApplication1.Models;
using WebApplication1.Persistance;

namespace WebApplication1.Services
{
    public class UserService
    {
        private readonly ILogger<UserController> _logger;
        private readonly DbContextUsers _context;

        public UserService(ILogger<UserController> logger, DbContextUsers context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllUsers(int pageNumber, int pageSize, string sortBy, string filter)
        {
            var query = _context.Users.Include(i => i.Roles).ToList();
            switch (sortBy)
            {
                case "name":
                    query = query.OrderBy(i => i.Name).ToList();
                    break;
                case "age":
                    query = query.OrderBy(i => i.Age).ToList();
                    break;
                default:
                    query = query.OrderBy(i => i.Id).ToList();
                    break;
            }

            if (!string.IsNullOrEmpty(filter))
            {
                var filters = filter.Split(',');

                foreach (var f in filters)
                {
                    var keyValue = f.Split(':');
                    var key = keyValue[0];
                    var value = keyValue[1];

                    switch (key)
                    {
                        case "name":
                            query = query.Where(i => i.Name.Contains(value)).ToList();
                            break;
                        case "age":
                            query = query.Where(i => i.Age == Convert.ToInt32(value)).ToList();
                            break;
                        case "role":
                            var role = _context.Roles.FirstOrDefault(i => i.Name.ToString().Contains(value));
                            query = query.Where(i => i.Roles == role).ToList();
                            break;
                    }
                }
            }
            var paginatedResult = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return paginatedResult;
        }

        public async Task<User> GetUserRoles(Guid userId)
        {
            try
            {
                var user = await _context.Users
                .Include(e => e.Roles)
                .FirstOrDefaultAsync(e => e.Id == userId);

                if (user != null)
                {
                    return user;
                }
                else new Exception("User not exist");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
            }
            return null;
        }

        public async Task AddUser(User user)
        {
            try
            {
                using var transaction = _context.Database.BeginTransaction();
                await _context.Users.AddAsync(user);
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
            }
        }

        public async Task AddRoleToUser(Guid userid, RoleName rolename)
        {
            try
            {
                using var transaction = _context.Database.BeginTransaction();

                var user = await _context.Users.Include(x => x.Roles).FirstOrDefaultAsync(x => x.Id == userid);
                var role = await _context.Roles.FirstOrDefaultAsync(x => x.Name == rolename);
                user.Roles.Add(role);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
            }
        }

        public async Task UpdateUser(Guid id, User user)
        {
            try
            {
                using var transaction = _context.Database.BeginTransaction();

                var oldUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
                if (oldUser == null)
                {
                    return;
                }
                _context.Remove(oldUser);
                await _context.SaveChangesAsync();

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
            }
        }

        public async Task DeleteUser(Guid id)
        {
            try
            {
                using var transaction = _context.Database.BeginTransaction();

                var oldUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
                if (oldUser == null)
                {
                    return;
                }
                _context.Remove(oldUser);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
            }
        }
    }
}
