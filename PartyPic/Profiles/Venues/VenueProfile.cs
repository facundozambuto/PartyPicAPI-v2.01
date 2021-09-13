using AutoMapper;
using PartyPic.DTOs.Venues;
using PartyPic.Models.Venues;

namespace PartyPic.Profiles.Venues
{
    public class VenueProfile : Profile
    {
        public VenueProfile()
        {
            CreateMap<Venue, VenueReadDTO>();
            CreateMap<VenueCreateDTO, Venue>();
            CreateMap<VenueGrid, VenueReadDTOGrid>();
            CreateMap<VenueReadDTOGrid, VenueGrid>();
            CreateMap<VenueUpdateDTO, Venue>();
            CreateMap<Venue, VenueUpdateDTO>();
            CreateMap<AllVenuesResponse, GetAllVenuesApiResponse>();
            CreateMap<GetAllVenuesApiResponse, AllVenuesResponse>();
            CreateMap<AllVenuesResponse, GetAllVenuesApiResponse>();
            CreateMap<GetAllVenuesApiResponse, AllVenuesResponse>();
            CreateMap<VenueGrid, GridVenueApiResponse>();
            CreateMap<GridVenueApiResponse, VenueGrid>();
            CreateMap<VenueReadDTOGrid, GridVenueApiResponse>();
            CreateMap<GridVenueApiResponse, VenueReadDTOGrid>();
            CreateMap<VenueApiResponse, Venue>();
            CreateMap<Venue, VenueApiResponse>();
            CreateMap<VenueApiResponse, VenueReadDTO>();
            CreateMap<VenueReadDTO, VenueApiResponse>();
        }
    }
}
