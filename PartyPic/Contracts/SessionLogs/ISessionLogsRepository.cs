using PartyPic.Models.SessionLogs;

namespace PartyPic.Contracts.SessionLogs
{
    public interface ISessionLogsRepository
    {
        void AddSessionLog(int userId, string actionType);
        AllSessionLogsResponse GetSessionLogs();
    }
}
