using System;
using System.ComponentModel.DataAnnotations;

namespace PartyPic.DTOs.Events
{
    public class EventUpdateDTO
    {
        [Required]
        [MaxLength(250)]
        public string Code { get; set; }
        [Required]
        [MaxLength(250)]
        public string Description { get; set; }
        [Required]
        public int VenueId { get; set; }
        public DateTime StartDatetime { get; set; }
        [Required]
        [MaxLength(250)]
        public string QRCode { get; set; }
        [Required]
        public bool Enabled { get; set; }
        [Required]
        public DateTime LastRequest { get; set; }
        [Required]
        public DateTime CreatedDatetime { get; set; }
    }
}
