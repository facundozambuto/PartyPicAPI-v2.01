using System;
using System.ComponentModel.DataAnnotations;

namespace PartyPic.Models.Venues
{
    public class Venue
    {
        [Key]
        [Required]
        public int VenueId { get; set; }
        [Required]
        [MaxLength(250)]
        public string Name { get; set; }
        [Required]
        [MaxLength(250)]
        public string Address { get; set; }
        [Required]
        public DateTime CreatedDatetime { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        [MaxLength(250)]
        public string Phone { get; set; }
    }
}
