using System;

namespace PartyPic.DTOs.SessionLogs
{
    public class SessionLogsReadDTO
    {
        public int SessionLogId { get; set; }
        public int UserId { get; set; }
        public string ActionType { get; set; }
        public DateTime CreatedDatetime { get; set; }
    }
}
