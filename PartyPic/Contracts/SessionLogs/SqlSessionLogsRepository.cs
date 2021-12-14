using PartyPic.Models.SessionLogs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PartyPic.Contracts.SessionLogs
{
    public class SqlSessionLogsRepository : ISessionLogsRepository
    {
        private readonly SessionLogsContext _sessionLogsContext;

        public SqlSessionLogsRepository(SessionLogsContext sessionLogsContext)
        {
            _sessionLogsContext = sessionLogsContext;
        }


        public void AddSessionLog(int userId, string actionType)
        {
            var sessionLog = new SessionLog
            {
                UserId = userId,
                ActionType = actionType,
                CreatedDatetime = DateTime.Now,
            };

            _sessionLogsContext.SessionLogs.Add(sessionLog);

            this.SaveChanges();
        }

        public AllSessionLogsResponse GetSessionLogs()
        {
            return new AllSessionLogsResponse
            {
                SessionLogs = _sessionLogsContext.SessionLogs.ToList()
            };
        }

        public bool SaveChanges()
        {
            return (_sessionLogsContext.SaveChanges() >= 0);
        }
    }
}
