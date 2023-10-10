using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Dtos;
using WebApplication1.Persistance;

namespace SportsCompetition.Validators
{
    public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
    {
        private readonly DbContextUsers _context; 
        public UpdateUserDtoValidator( DbContextUsers context )
        {
            _context = context;
        }

        public UpdateUserDtoValidator()
        {
            RuleFor(e => e.Password)
                .NotEmpty()
                .MinimumLength(8)
                .Matches("^.*(?=.{8,})(?=.*[a-zA-Z])(?=.*\\d)(?=.*[!#$%&? \"]).*$");

            RuleFor(e => e.Name)
                .NotEmpty()
                .MinimumLength(2);

            RuleFor(e => e.Age)
                .NotEmpty()
                .GreaterThan(0);

            RuleFor(e => e.Email)
                .NotEmpty()
                .EmailAddress()
                .MustAsync(CheckValue);
        }

        public async Task<bool> CheckValue(string v, CancellationToken token)
        {
            var username= await _context.Users.FirstOrDefaultAsync(u => u.Email == v, token);
                if (username == null) { return true; };
            return false;
        }
    }
}
