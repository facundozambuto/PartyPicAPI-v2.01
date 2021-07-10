using System;
using System.ComponentModel.DataAnnotations;

namespace PartyPic.DTOs.Categories
{
    public class CategoryUpdateDTO
    {
        [Required]
        [MaxLength(250)]
        public string Description { get; set; }
    }
}
