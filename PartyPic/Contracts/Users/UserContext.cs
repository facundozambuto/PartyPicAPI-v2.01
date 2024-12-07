using Microsoft.EntityFrameworkCore;
using PartyPic.Models.Users;

namespace PartyPic.Contracts.Users
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base (options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
            .ToTable("Users");
        }

        public DbSet<User> Users { get; set; }
    }
}
