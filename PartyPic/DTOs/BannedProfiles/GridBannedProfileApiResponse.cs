using PartyPic.Models.Common;
using System.Collections.Generic;

namespace PartyPic.DTOs.BannedProfiles
{
    public class GridBannedProfileApiResponse : ApiResponse
    {
        public List<BannedProfileReadDTO> Rows { get; set; }
        public int Total { get; set; }
        public int RowCount { get; set; }
        public int Current { get; set; }
    }
}
