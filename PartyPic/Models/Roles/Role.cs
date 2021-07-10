using System;
using System.ComponentModel.DataAnnotations;

namespace PartyPic.Models.Roles
{
    public class Role
    {
        [Key]
        [Required]
        public int RoleId { get; set; }
        [Required]
        [MaxLength(250)]
        public string Description { get; set; }
        [Required]
        public DateTime CreatedDatetime { get; set; }
    }
}
