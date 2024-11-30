namespace PartyPic.Models.Payments
{
    using PartyPic.Models.Events;
    using PartyPic.Models.Users;
    using System;

    public class Payment
    {
        public int PaymentId { get; set; }
        public int UserId { get; set; }
        public int? EventId { get; set; }
        public string PaymentType { get; set; }
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; }
        public string MercadoPagoId { get; set; }
        public DateTime CreatedDatetime { get; set; } = DateTime.UtcNow;
        public User User { get; set; }
        public Event Event { get; set; }
    }
}
