using PartyPic.DTOs.Subscriptions;
using System.Collections.Generic;

namespace PartyPic.Models.Subscriptions
{
    public class AllSubscriptionsResponse
    {
        public List<SubscriptionReadDTO> Subscriptions { get; set; }
    }
}
