using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PartyPic.DTOs.SessionLogs
{
    public class SessionLogsCreateDTO
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        [MaxLength(250)]
        public string ActionType { get; set; }
        [Required]
        public DateTime CreatedDatetime { get; set; }
    }
}
