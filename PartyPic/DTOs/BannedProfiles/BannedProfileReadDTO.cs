using System;

namespace PartyPic.DTOs.BannedProfiles
{
    public class BannedProfileReadDTO
    {
        public string ProfileId { get; set; }
        public string BannedName { get; set; }
        public int UserId { get; set; }
        public int EventId { get; set; }
        public DateTime BanDatetime { get; set; }
        public string UserName { get; set; }
        public string EventName { get; set; }
    }
}
