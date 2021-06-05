using Microsoft.EntityFrameworkCore;
using PartyPic.Models.Events;

namespace PartyPic.Contracts.Events
{
    public class EventContext : DbContext
    {
        public EventContext(DbContextOptions<EventContext> options) : base(options)
        {

        }

        public DbSet<Event> Events { get; set; }
    }
}
