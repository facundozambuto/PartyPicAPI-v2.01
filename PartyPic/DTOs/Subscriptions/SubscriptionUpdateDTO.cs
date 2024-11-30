namespace PartyPic.DTOs.Subscriptions
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class SubscriptionUpdateDTO
    {
        [Required]
        [MaxLength(250)]
        public string PlanType { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public string MercadoPagoId { get; set; }
        [Required]
        public DateTime CreatedDatetime { get; set; }
    }
}
