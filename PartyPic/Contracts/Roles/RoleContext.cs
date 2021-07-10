using Microsoft.EntityFrameworkCore;
using PartyPic.Models.Roles;

namespace PartyPic.Contracts.Roles
{
    public class RoleContext : DbContext
    {
        public RoleContext(DbContextOptions<RoleContext> options) : base(options)
        {

        }

        public DbSet<Role> Roles { get; set; }
    }
}
