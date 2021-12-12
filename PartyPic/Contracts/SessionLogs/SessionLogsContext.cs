using Microsoft.EntityFrameworkCore;
using PartyPic.Models.SessionLogs;

namespace PartyPic.Contracts.SessionLogs
{
    public class SessionLogsContext : DbContext
    {
        public SessionLogsContext(DbContextOptions<SessionLogsContext> options) : base(options)
        {

        }

        public DbSet<SessionLog> SessionLogs { get; set; }
    }
}
