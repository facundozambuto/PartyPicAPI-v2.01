namespace PartyPic.DTOs.Subscriptions
{
    using PartyPic.Models.Common;
    using System.Collections.Generic;

    public class GridSubscriptionApiResponse : ApiResponse
    {
        public List<SubscriptionReadDTO> Rows { get; set; }
        public int Total { get; set; }
        public int RowCount { get; set; }
        public int Current { get; set; }
    }
}
