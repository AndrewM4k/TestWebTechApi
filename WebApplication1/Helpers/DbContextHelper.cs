using Microsoft.EntityFrameworkCore;

namespace SportsCompetition.Helpers
{
    public class DbContextHelper
    {
        public static async Task ApplyMigrations(DbContext context)
        {
            await context.Database.MigrateAsync();
        }
    }
}
