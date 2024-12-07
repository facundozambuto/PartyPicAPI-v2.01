using System.Collections.Generic;

namespace PartyPic.DTOs.Plans
{
    public class PlanReadDTOGrid
    {
        public List<PlanReadDTO> Rows { get; set; }
        public int Total { get; set; }
        public int RowCount { get; set; }
        public int Current { get; set; }
    }
}
