using PartyPic.Models.Common;
using System.Collections.Generic;

namespace PartyPic.DTOs.BannedProfiles
{
    public class GetAllBannedProfileResponse : ApiResponse
    {
        public List<BannedProfileReadDTO> BannedProfiles { get; set; }
    }
}
