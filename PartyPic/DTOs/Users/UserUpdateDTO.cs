using System;
using System.ComponentModel.DataAnnotations;

namespace PartyPic.DTOs.Users
{
    public class UserUpdateDTO
    {
        [Required]
        [MaxLength(250)]
        public string Name { get; set; }
        [Required]
        public int RoleId { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public DateTime CreatedDatetime { get; set; }
        [Required]
        [MaxLength(250)]
        public string Email { get; set; }
        [MaxLength(250)]
        public string Password { get; set; }
        [Required]
        [MaxLength(250)]
        public string Phone { get; set; }
        [Required]
        [MaxLength(250)]
        public string Address { get; set; }
        [Required]
        [MaxLength(250)]
        public string Cuil { get; set; }
        [Required]
        [MaxLength(250)]
        public string MobilePhone { get; set; }
    }
}
