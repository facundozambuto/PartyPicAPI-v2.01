using PartyPic.Models.SessionLogs;
using System;
using System.Collections.Generic;

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

        public List<SessionLog> GetSessionLogs()
        {
            throw new System.NotImplementedException();
        }

        public bool SaveChanges()
        {
            return (_sessionLogsContext.SaveChanges() >= 0);
        }
    }
}
