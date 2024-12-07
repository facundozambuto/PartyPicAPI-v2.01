namespace PartyPic.Models.Plans
{
    using System;

    public class PriceHistory
    {
        public int Id { get; set; }
        public int PlanId { get; set; }
        public decimal Price { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedDatetime { get; set; }

        public Plan Plan { get; set; }
    }
}
