using Microsoft.EntityFrameworkCore;
using PartyPic.Models.Users;

namespace PartyPic.Contracts.Users
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base (options)
        {

        }

        public DbSet<User> Users { get; set; }
    }
}
