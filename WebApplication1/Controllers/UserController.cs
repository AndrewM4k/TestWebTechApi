using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using WebApplication1.Dtos;
using WebApplication1.Enums;
using WebApplication1.Models;
using WebApplication1.Persistance;
using WebApplication1.Repositories;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly DbContextUsers _context;
        private readonly IMapper _mapper;
        private readonly UserService _userService;
        private readonly IValidator<AddUserDto> _validator;
        private readonly IUserRepository _userRepository;

        public UserController(DbContextUsers context, IMapper mapper, UserService userService, IValidator<AddUserDto> validator, IUserRepository userRepository)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
            _validator = validator;
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<User>> Get(int pageNumber, int pageSize, string sortBy, string filter, CancellationToken cancellationToken)
        {
            //return await _userService.GetAllUsers(pageNumber, pageSize, sortBy, filter);

            return await _userRepository.GetAllAsync(new Queries.UserQuery(), cancellationToken);
        }

        [HttpGet("{id:Guid}")]
        public async Task<User> GetUserRolesById(Guid id)
        {
            return await _userService.GetUserRoles(id);
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(AddUserDto dto)
        {
            var validateresult = await _validator.ValidateAsync(dto);
            if (!validateresult.IsValid)
            {
                return BadRequest(validateresult.Errors.ToList());
            }
            
            var user = _mapper.Map<User>(dto);
            await _userService.AddUser(user);
            return Ok();
        }

        //[HttpPatch]
        //public async Task<IActionResult> AddRoleToUser(Guid userid, RoleName role)
        //{
        //    await _userService.AddRoleToUser(userid, role);
        //    return Ok();
        //}

        [HttpPatch]
        public async Task<IActionResult> UpdateUser(Guid id, AddUserDto dto)
        {
            var validateresult = await _validator.ValidateAsync(dto);
            if (!validateresult.IsValid)
            {
                return BadRequest(validateresult.Errors.ToList());
            }

            var user = _mapper.Map<User>(dto);
            await _userService.UpdateUser(id, user);
            return Ok();
        }

        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            await _userService.DeleteUser(id);
            return Ok();
        }

    }
}
