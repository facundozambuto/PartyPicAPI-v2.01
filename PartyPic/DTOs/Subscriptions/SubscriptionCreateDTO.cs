namespace PartyPic.DTOs.Subscriptions
{
    using System.ComponentModel.DataAnnotations;

    public class SubscriptionCreateDTO
    {
        [Required]
        public bool IsAutoRenew { get; set; }
        [Required]
        public int PlanId { get; set; }
    }
}
