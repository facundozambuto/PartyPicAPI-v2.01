using System.Collections.Generic;

namespace PartyPic.DTOs.Roles
{
    public class RoleReadDTOGrid
    {
        public List<RoleReadDTO> Rows { get; set; }
        public int Total { get; set; }
        public int RowCount { get; set; }
        public int Current { get; set; }
    }
}
