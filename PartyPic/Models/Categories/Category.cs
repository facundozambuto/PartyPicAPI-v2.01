using System;
using System.ComponentModel.DataAnnotations;

namespace PartyPic.Models.Categories
{
    public class Category
    {
        [Key]
        [Required]
        public int CategoryId { get; set; }
        [Required]
        [MaxLength(250)]
        public string Description { get; set; }
        [Required]
        public DateTime CreatedDatetime { get; set; }
    }
}
