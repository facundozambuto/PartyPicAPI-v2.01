using Microsoft.EntityFrameworkCore;
using PartyPic.Models.BannedProfile;

namespace PartyPic.Contracts.BannedProfiles
{
    public class BannedProfileContext : DbContext
    {
        public BannedProfileContext(DbContextOptions<BannedProfileContext> options) : base(options)
        {

        }

        public DbSet<BannedProfile> BannedProfiles { get; set; }
    }
}
