using System.ComponentModel.DataAnnotations;

namespace PartyPic.DTOs.Plans
{
    public class PlanUpdateDTO
    {
        [Required]
        [MaxLength(250)]
        public string Description { get; set; }

        [Required]
        [MaxLength(250)]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }
    }
}
