using PartyPic.DTOs.Plans;
using System.Collections.Generic;

namespace PartyPic.Models.Plans
{
    public class AllPlansResponse
    {
        public List<PlanReadDTO> Plans { get; set; }
    }
}
