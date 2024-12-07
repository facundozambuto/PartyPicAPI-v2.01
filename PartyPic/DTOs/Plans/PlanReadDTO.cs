using System;

namespace PartyPic.DTOs.Plans
{
    public class PlanReadDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDatetime { get; set; }
        public decimal LatestPrice { get; set; }
    }
}
