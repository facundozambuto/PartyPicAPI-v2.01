using PartyPic.Models.Users;
using System;
using System.ComponentModel.DataAnnotations;

namespace PartyPic.Models.Subscriptions
{
    public class Subscription
    {
        [Key]
        [Required]
        public int SubscriptionId { get; set; }
        [Required]
        public int UserId { get; set; }
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
        [MaxLength(250)]
        public string MercadoPagoId { get; set; }
        [Required]
        public DateTime CreatedDatetime { get; set; }
        public User User { get; set; }
    }
}
