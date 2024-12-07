namespace PartyPic.Models.Plans
{
    using System;
    using System.Collections.Generic;

    public class Plan
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDatetime { get; set; }
        public ICollection<PriceHistory> PriceHistories { get; set; }
    }
}
