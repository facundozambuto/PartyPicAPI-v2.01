using PartyPic.Models.SessionLogs;
using System.Collections.Generic;

namespace PartyPic.Contracts.SessionLogs
{
    public interface ISessionLogsRepository
    {
        void AddSessionLog(int userId, string actionType);
        List<SessionLog> GetSessionLogs();
    }
}
