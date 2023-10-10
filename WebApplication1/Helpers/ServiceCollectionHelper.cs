using Microsoft.EntityFrameworkCore;

namespace SportsCompetition.Helpers
{
    public static class ServiceCollectionHelper
    {
        public static async Task ApplyMigarationForDbContext<T>(this IServiceProvider services) where T : DbContext
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetService<T>();
            await DbContextHelper.ApplyMigrations(context);
        }
    }
}
