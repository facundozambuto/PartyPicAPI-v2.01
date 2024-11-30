namespace PartyPic.DTOs.Payments
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class PaymentUpdateDTO
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        [MaxLength(250)]
        public int? EventId { get; set; }
        [Required]
        public string PaymentType { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public string PaymentStatus { get; set; }
        [Required]
        public string MercadoPagoId { get; set; }
    }
}
