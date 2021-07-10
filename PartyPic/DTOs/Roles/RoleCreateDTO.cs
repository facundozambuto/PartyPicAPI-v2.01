using System;
using System.ComponentModel.DataAnnotations;

namespace PartyPic.DTOs.Roles
{
    public class RoleCreateDTO
    {
        [Required]
        [MaxLength(250)]
        public string Description { get; set; }
        [Required]
        public DateTime CreatedDatetime { get; set; }
    }
}
