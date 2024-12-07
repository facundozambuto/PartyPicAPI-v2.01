using System;
using System.ComponentModel.DataAnnotations;

namespace PartyPic.DTOs.Plans
{
    public class PlanCreateDTO
    {
        [Required]
        [MaxLength(250)]
        public string Name { get; set; }
        [Required]
        [MaxLength(250)]
        public string Description { get; set; }
        [Required]
        public decimal InitialPrice { get; set; }
        [Required]
        public DateTime CreatedDatetime { get; set; }
    }
}
