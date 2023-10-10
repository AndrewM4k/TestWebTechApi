using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApplication1.Enums;
using WebApplication1.Models;

namespace WebApplication1.Persistance
{
    public class DbContextUsers : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public DbContextUsers(DbContextOptions<DbContextUsers> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<RefreshToken>()
                .HasKey(t => new { t.UserId, t.Token });

            builder.Entity<User>()
                .Property(u => u.Gender)
                .HasConversion(new EnumToStringConverter<Gender>());

            builder.Entity<Role>()
                .Property(r => r.Name)
                .HasConversion(new EnumToStringConverter<RoleName>());

            builder.Entity<User>()
                .HasMany(u => u.Roles)
                .WithMany(r => r.Users)
                .UsingEntity<UserRole>(
                    user => user
                        .HasOne<Role>()
                        .WithMany(u => u.UserRole)
                        .HasForeignKey(u=> u.RoleId),
                    role => role
                        .HasOne<User>()
                        .WithMany(p => p.UserRole)
                        .HasForeignKey(o => o.UserId)
                    );

            builder.Entity<User>()
                .HasIndex(x => x.Username)
                .IsUnique();

            builder.Entity<User>()
                .HasIndex(x => x.Email)
                .IsUnique();
        }
    }
}
