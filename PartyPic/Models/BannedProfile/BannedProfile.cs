using System;
using System.ComponentModel.DataAnnotations;

namespace PartyPic.Models.BannedProfile
{
    public class BannedProfile
    {
        [Required]
        [Key]
        public int BannedId { get; set; }
        [Required]
        public string ProfileId { get; set; }
        public string BannedName { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int EventId { get; set; }
        [Required]
        public DateTime BanDatetime { get; set; }
    }
}
