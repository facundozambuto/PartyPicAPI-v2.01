using System;
using System.ComponentModel.DataAnnotations;

namespace PartyPic.DTOs.Events
{
    public class EventCreateDTO
    {
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
    }
}
