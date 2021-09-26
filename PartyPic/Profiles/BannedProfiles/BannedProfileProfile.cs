using AutoMapper;
using PartyPic.DTOs.BannedProfiles;
using PartyPic.Models.BannedProfile;
using PartyPic.Models.BannedProfiles;

namespace PartyPic.BannedProfiles.BannedProfiles
{
    public class BannedProfileProfile : Profile
    {
        public BannedProfileProfile()
        {
            CreateMap<BannedProfile, BannedProfileReadDTO>();
            CreateMap<BannedProfileGrid, BannedProfileReadDTOGrid>();
            CreateMap<BannedProfileReadDTOGrid, BannedProfileGrid>();
            CreateMap<GridBannedProfileApiResponse, BannedProfileGrid>();
            CreateMap<BannedProfileGrid, GridBannedProfileApiResponse>(); 
            CreateMap<BannedProfileResponse, BannedProfileReadDTO>();
            CreateMap<BannedProfileReadDTO, BannedProfileResponse>();
            CreateMap<GetAllBannedProfileResponse, AllBannedProfileResponse>();
            CreateMap<AllBannedProfileResponse, GetAllBannedProfileResponse>();
        }
    }
}
