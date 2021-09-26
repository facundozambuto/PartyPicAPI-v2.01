using PartyPic.DTOs.BannedProfiles;
using PartyPic.Models.BannedProfile;
using PartyPic.Models.BannedProfiles;
using PartyPic.Models.Common;

namespace PartyPic.Contracts.BannedProfiles
{
    public interface IBannedProfileRepository
    {
        AllBannedProfileResponse GetAllBannedProfiles();
        BannedProfileReadDTO GetBannedProfileById(string bannedProfileId);
        BannedProfile BlockProfile(BannedProfile bannedProfile);
        bool SaveChanges();
        void UnblockProfile(string bannedProfileId);
        BannedProfileGrid GetAllBannedProfilesForGrid(GridRequest gridRequest);
    }
}
