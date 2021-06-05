using PartyPic.Models.Common;
using System.Collections.Generic;

namespace PartyPic.Models.Users
{
    public class UserGrid : ApiResponse
    {
        public List<User> Rows { get; set; }
        public int Total { get; set; }
        public int RowCount { get; set; }
        public int Current { get; set; }
    }
}
