using AutoMapper;
using PartyPic.DTOs;
using PartyPic.Models.Reports;

namespace PartyPic.Profiles.Venues
{
    public class Reportsrofile : Profile
    {
        public Reportsrofile()
        {
            CreateMap<ReportsResponse, GetReportsApiResponse>();
            CreateMap<GetReportsApiResponse, ReportsResponse>();
        }
    }
}
