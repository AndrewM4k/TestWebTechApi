using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Dtos;
using WebApplication1.Enums;
using WebApplication1.Models;
using WebApplication1.Persistance;
using WebApplication1.Queries;
using WebApplication1.Repositories;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly DbContextUsers _context;
        private readonly IMapper _mapper;
        private readonly UserService _userService;
        private readonly IValidator<AddUserDto> _addUserDtoValidator;
        private readonly IValidator<UpdateUserDto> _updateUserDtoValidator;
        private readonly IUserRepository _userRepository;

        public UserController(DbContextUsers context, IMapper mapper, UserService userService, IValidator<AddUserDto> validator, IUserRepository userRepository, IValidator<UpdateUserDto> updateValidator)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
            _addUserDtoValidator = validator;
            _userRepository = userRepository;
            _updateUserDtoValidator = updateValidator;
        }

        [HttpGet]
        public async Task<IEnumerable<GetUserDto>> Get([FromQuery] UserQuery query, CancellationToken cancellationToken)
        {
            var users = await _userService.GetAllAsync(query, cancellationToken);
            return users;
        }

        [HttpGet("{id:Guid}")]
        public async Task<GetUserDto> GetUserWithRolesById(Guid id, CancellationToken cancellationToken)
        {
            return await _userService.GetUserWithRolesAsync(id, cancellationToken);
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(AddUserDto dto, CancellationToken cancellationToken)
        {
            var validateresult = await _addUserDtoValidator.ValidateAsync(dto);
            if (!validateresult.IsValid)
            {
                return BadRequest(validateresult.Errors.ToList());
            }

            var user = _mapper.Map<User>(dto);
            await _userService.AddUser(user, dto.Password, cancellationToken);
            return Ok();
        }

        [HttpPatch("addRole/{userId}/{roleName}")]
        public async Task<IActionResult> AddRoleToUser(Guid userId, RoleName roleName, CancellationToken cancellationToken)
        {
            await _userService.AddRoleToUserAsync(userId, roleName, cancellationToken);
            return Ok();
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateUser(Guid id, UpdateUserDto dto, CancellationToken cancellationToken)
        {
            var validateresult = await _updateUserDtoValidator.ValidateAsync(dto);
            if (!validateresult.IsValid)
            {
                return BadRequest(validateresult.Errors.ToList());
            }

            var user = _mapper.Map<User>(dto);
            await _userService.UpdateUserAsync(id, user, dto.Password, cancellationToken);
            return Ok();
        }

        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteUser(Guid id, CancellationToken cancellationToken)
        {
            await _userService.DeleteUserAsync(id, cancellationToken);
            return Ok();
        }

    }
}
