using System;

namespace PartyPic.DTOs.Plans
{
    public class PlanPriceHistoryDTO
    {
        public string PlanName { get; set; }
        public decimal Price { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
