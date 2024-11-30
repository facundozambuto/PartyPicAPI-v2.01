using PartyPic.Models.Payments;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PartyPic.Models.Events
{
    public class Event
    {
        [Key]
        [Required]
        public int EventId { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        [MaxLength(250)]
        public string Name { get; set; }
        [Required]
        [MaxLength(250)]
        public string Description { get; set; }
        [Required]
        public int VenueId { get; set; }
        public DateTime StartDatetime { get; set; }
        public string QRCode { get; set; }
        [Required]
        public bool Enabled { get; set; }
        public DateTime? LastRequest { get; set; }
        public DateTime? CreatedDatetime { get; set; }
        public int CategoryId { get; set; }
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
