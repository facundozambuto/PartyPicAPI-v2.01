using PartyPic.Models.Common;
using System.Collections.Generic;

namespace PartyPic.DTOs.Users
{
    public class GridUserApiResponse : ApiResponse
    {
        public List<UserReadDTO> Rows { get; set; }
        public int Total { get; set; }
        public int RowCount { get; set; }
        public int Current { get; set; }
    }
}
