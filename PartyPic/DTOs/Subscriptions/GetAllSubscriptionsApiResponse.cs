namespace PartyPic.DTOs.Subscriptions
{
    using PartyPic.Models.Common;
    using System.Collections.Generic;

    public class GetAllSubscriptionsApiResponse : ApiResponse
    {
        public List<SubscriptionReadDTO> Subscriptions { get; set; }
    }
}
