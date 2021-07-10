using System;
using System.ComponentModel.DataAnnotations;

namespace PartyPic.DTOs.Categories
{
    public class CategoryCreateDTO
    {
        [Required]
        [MaxLength(250)]
        public string Description { get; set; }
        [Required]
        public DateTime CreatedDatetime { get; set; }
    }
}
