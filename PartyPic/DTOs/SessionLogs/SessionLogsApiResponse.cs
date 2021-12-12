using PartyPic.Models.Common;

namespace PartyPic.DTOs.SessionLogs
{
    public class SessionLogsApiResponse : ApiResponse
    {
        public int SessionLogId { get; set; }
        public int UserId { get; set; }
        public string ActionType { get; set; }
    }
}
