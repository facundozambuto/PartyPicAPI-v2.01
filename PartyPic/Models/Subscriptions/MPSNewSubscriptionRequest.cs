using System;

namespace PartyPic.Models.Subscriptions
{
    public class MPSNewSubscriptionRequest
    {
        public Guid MarketReference { get; set; }
        public bool IsAutoRenew { get; set; }
        public string PlanType { get; set; }
        public double Amount { get; set; }
        public string UserEmail { get; set; }
    }
}
