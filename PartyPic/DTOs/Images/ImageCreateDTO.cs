using System;
using System.ComponentModel.DataAnnotations;

namespace PartyPic.DTOs.Images
{
    public class ImageCreateDTO
    {
        [Required]
        [MaxLength(250)]
        public string Path { get; set; }
        [Required]
        public int EventId { get; set; }
        [Required]
        public string ProfileId { get; set; }
        [Required]
        public DateTime CreatedDatetime { get; set; }
        [Required]
        [MaxLength(250)]
        public string Comment { get; set; }
        [Required]
        [MaxLength(250)]
        public string ProfileName { get; set; }
        [Required]
        [MaxLength(250)]
        public string ProfileImageUrl { get; set; }
    }
}
