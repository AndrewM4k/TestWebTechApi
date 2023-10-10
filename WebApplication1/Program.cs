using ApiWithEF.Common;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Extencions;
using WebApplication1.Services;
using FluentValidation;
using WebApplication1.Persistance;
using Microsoft.OpenApi.Models;
using SportsCompetition.Helpers;
using WebApplication1.Repositories;

namespace WebApplication1
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddScoped<UserService>();

            builder.Services.AddIdentityServicer(builder.Configuration);
            builder.Services.AddScoped<TokenService>();
            builder.Services.AddScoped<RefreshTokenService>();
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            builder.Services.AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Scoped);

            builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
            // Add services to the container.
            builder.Services.AddDbContext<DbContextUsers>(options =>
               options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddControllersWithViews();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            }
            );

            var app = builder.Build();

            await app.Services.ApplyMigarationForDbContext<DbContextUsers>();

            await app.Services.SeedDataContext();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}