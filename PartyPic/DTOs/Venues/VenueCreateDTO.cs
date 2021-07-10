using System;
using System.ComponentModel.DataAnnotations;

namespace PartyPic.DTOs.Venues
{
    public class VenueCreateDTO
    {
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
