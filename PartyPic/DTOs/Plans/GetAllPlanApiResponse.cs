using PartyPic.Models.Common;
using System.Collections.Generic;

namespace PartyPic.DTOs.Plans
{
    public class GetAllPlansApiResponse : ApiResponse
    {
        public List<PlanReadDTO> Plans { get; set; }
    }
}
