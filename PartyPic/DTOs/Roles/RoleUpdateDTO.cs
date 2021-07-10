using System.ComponentModel.DataAnnotations;

namespace PartyPic.DTOs.Roles
{
    public class RoleUpdateDTO
    {
        [Required]
        [MaxLength(250)]
        public string Description { get; set; }
    }
}
