using PartyPic.Models.Common;

namespace PartyPic.DTOs.Plans
{
    public class PlanApiResponse : ApiResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal LatestPrice { get; set; }
    }
}
