using PartyPic.DTOs.Plans;
using PartyPic.Models.Common;
using System.Collections.Generic;

namespace PartyPic.Models.Plans
{
    public class PlanGrid : ApiResponse
    {
        public List<PlanReadDTO> Rows { get; set; }
        public int Total { get; set; }
        public int RowCount { get; set; }
        public int Current { get; set; }
    }
}
