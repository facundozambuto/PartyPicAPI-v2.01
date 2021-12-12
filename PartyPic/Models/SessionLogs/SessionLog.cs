using System;
using System.ComponentModel.DataAnnotations;

namespace PartyPic.Models.SessionLogs
{
    public class SessionLog
    {
        [Key]
        [Required]
        public int SessionLogId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        [MaxLength(250)]
        public string ActionType { get; set; }
        [Required]
        public DateTime CreatedDatetime { get; set; }
    }
}
