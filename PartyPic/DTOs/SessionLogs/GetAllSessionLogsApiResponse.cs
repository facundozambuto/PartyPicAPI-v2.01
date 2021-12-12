using PartyPic.Models.Common;
using System.Collections.Generic;

namespace PartyPic.DTOs.SessionLogs
{
    public class GetAllSessionLogsApiResponse: ApiResponse
    {
        public List<SessionLogsReadDTO> SessionLogs { get; set; }
    }
}
