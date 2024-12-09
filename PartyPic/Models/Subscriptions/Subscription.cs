using PartyPic.Models.Plans;
using PartyPic.Models.Users;
using System;

namespace PartyPic.Models.Subscriptions
{
    public class Subscription
    {
        public int SubscriptionId { get; set; }
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public string MercadoPagoId { get; set; }
        public DateTime CreatedDatetime { get; set; }
        public bool IsAutoRenew { get; set; }
        public int? PlanId { get; set; }
        public Guid MarketReference { get; set; }
        public Plan Plan { get; set; }
        public User User { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime? CancelledDate { get; set; }
    }
}
