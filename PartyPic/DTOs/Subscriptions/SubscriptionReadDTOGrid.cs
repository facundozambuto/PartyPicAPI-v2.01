namespace PartyPic.DTOs.Subscriptions
{
    using System.Collections.Generic;

    public class SubscriptionReadDTOGrid
    {
        public List<SubscriptionReadDTO> Rows { get; set; }
        public int Total { get; set; }
        public int RowCount { get; set; }
        public int Current { get; set; }
    }
}
