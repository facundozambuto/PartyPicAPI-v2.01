namespace PartyPic.DTOs.Subscriptions
{
    using PartyPic.Models.Common;
    using System;

    public class SubscriptionApiResponse : ApiResponse
    {
        public int SubscriptionId { get; set; }
        public int UserId { get; set; }
        public string PlanType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public string MercadoPagoId { get; set; }
        public DateTime CreatedDatetime { get; set; }
    }
}