using PartyPic.Models.Common;
using System.Collections.Generic;

namespace PartyPic.Models.Subscriptions
{
    public class SubscriptionGrid : ApiResponse
    {
        public List<Subscription> Rows { get; set; }
        public int Total { get; set; }
        public int RowCount { get; set; }
        public int Current { get; set; }
    }
}
