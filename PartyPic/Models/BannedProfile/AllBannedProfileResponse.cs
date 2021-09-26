using PartyPic.DTOs.BannedProfiles;
using System.Collections.Generic;

namespace PartyPic.Models.BannedProfiles
{
    public class AllBannedProfileResponse
    {
        public List<BannedProfileReadDTO> BannedProfiles { get; set; }
    }
}
