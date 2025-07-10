using Microsoft.EntityFrameworkCore;
using PlayerVault.Models;

namespace PlayerVault.Data
{
    public class PlayerVaultContext : DbContext
    {
        public PlayerVaultContext(DbContextOptions<PlayerVaultContext> options)
            : base(options)
        {
        }

        public DbSet<Player> Players { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed admin user
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                Username = "admin",
                Email = "admin@playervault.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Role = "Admin",
                IsVerified = true
            });
        }
    }
}
