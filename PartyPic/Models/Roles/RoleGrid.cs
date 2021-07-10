using PartyPic.Models.Common;
using System.Collections.Generic;

namespace PartyPic.Models.Roles
{
    public class RoleGrid : ApiResponse
    {
        public List<Role> Rows { get; set; }
        public int Total { get; set; }
        public int RowCount { get; set; }
        public int Current { get; set; }
    }
}
