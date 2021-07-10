using Microsoft.EntityFrameworkCore;
using PartyPic.Models.Venues;

namespace PartyPic.Contracts.Venues
{
    public class VenueContext : DbContext
    {
        public VenueContext(DbContextOptions<VenueContext> options) : base(options)
        {

        }

        public DbSet<Venue> Venues { get; set; }
    }
}
