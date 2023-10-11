using ApiWithEF.Common;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using WebApplication1.Controllers;
using WebApplication1.Dtos;
using WebApplication1.Enums;
using WebApplication1.Models;
using WebApplication1.Queries;
using WebApplication1.Repositories;

namespace WebApplication1.Services
{
    public class UserService
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHashingService _passwordHashingService;
        private readonly IMapper _mapper;

        public UserService(ILogger<UserController> logger, IUserRepository userRepository, IPasswordHashingService passwordHashingService, IMapper mapper)
        {
            _logger = logger;
            _userRepository = userRepository;
            _passwordHashingService = passwordHashingService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GetUserDto>> GetAllAsync(UserQuery userQuery, CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetAllAsync(userQuery, cancellationToken);
            var dtos = _mapper.Map<IEnumerable<GetUserDto>>(users);

            return dtos;
        }

        public async Task<GetUserDto> GetUserWithRolesAsync(Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _userRepository.GetUserWithRolesById(userId, cancellationToken);
                //конветировать в GetUserDto, проверить что бы в нем были роли
                var users = _mapper.Map<GetUserDto>(data);
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return null;
            }
        }

        public async Task AddUser(User user, string password, CancellationToken cancellationToken)
        {
            try
            {
                user.PasswordHash = _passwordHashingService.HashPassword(password);
                await _userRepository.AddUserAsync(user, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occured");
            }
        }

        public async Task AddRoleToUserAsync(Guid userId, RoleName rolename, CancellationToken cancellationToken)
        {
            try
            {
                await _userRepository.AddRoleToUserAsync(userId, rolename, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occured");
            }
        }

        public async Task UpdateUserAsync(Guid id, User user, string password, CancellationToken cancellationToken)
        {
            try
            {
                user.PasswordHash = _passwordHashingService.HashPassword(password);
                await _userRepository.UpdateUserAsync(id, user, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occured");
            }
        }

        public async Task DeleteUserAsync(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                await _userRepository.RemoveAsync(id, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occured");
            }
        }
    }
}
