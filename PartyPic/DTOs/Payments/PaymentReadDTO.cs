namespace PartyPic.DTOs.Payments
{
    using System;

    public class PaymentReadDTO
    {
        public int PaymentId { get; set; }
        public int UserId { get; set; }
        public int? EventId { get; set; }
        public string PaymentType { get; set; }
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; }
        public string MercadoPagoId { get; set; }
        public DateTime CreatedDatetime { get; set; } = DateTime.UtcNow;
    }
}
