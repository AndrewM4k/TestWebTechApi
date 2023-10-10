using Microsoft.AspNetCore.Identity;
using WebApplication1.Enums;
using WebApplication1.Models;
using WebApplication1.Persistance;

namespace WebApplication1
{
    public static class Seed
    {
        public static async Task SeedDataContext(this IServiceProvider services)
        {
            using (var scope = services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DbContextUsers>();

                if (!context.Roles.Any())
                {
                    var roles = new List<Role>()
                    {
                        new() { Name = RoleName.SuperAdmin, Id = new Guid() },
                        new() { Name = RoleName.Admin, Id = new Guid() },
                        new() { Name = RoleName.Support, Id = new Guid() },
                        new() { Name = RoleName.User, Id = new Guid() }
                    };
                    await context.Roles.AddRangeAsync(roles);
                    await context.SaveChangesAsync();
                }

                if (!context.Users.Any())
                {
                    Guid id = Guid.NewGuid();

                    var users = new List<User>()
                    {
                        new User()
                        {
                            Name = RoleName.SuperAdmin.ToString(),
                            Email = "andrey.mar4uk2011@yandex.ru",
                            Age = 0,
                            Username = RoleName.SuperAdmin.ToString(),
                            Gender = Gender.Male,
                            Password = "Password04$SuperAdmin",
                            Roles = new List<Role>(){ context.Roles.FirstOrDefault(i => i.Name == RoleName.SuperAdmin) }
                        },
                        new User()
                        {
                            Name = RoleName.Admin.ToString(),
                            Email = "andrew.mar4uk.dev@yandex.ru",
                            Age = 0,
                            Username = RoleName.Admin.ToString(),
                            Gender = Gender.Male,
                            Password = "Password04$Admin",
                            Roles = new List<Role>(){ context.Roles.FirstOrDefault(i => i.Name == RoleName.Admin) }
                        },
                    };
                    await context.Users.AddRangeAsync(users);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
