using AutoMapper;
using PartyPic.DTOs.SessionLogs;
using PartyPic.Models.SessionLogs;

namespace PartyPic.Profiles.SessionLogs
{
    public class SessionLogsProfile : Profile
    {
        public SessionLogsProfile()
        {
            CreateMap<SessionLog, SessionLogsReadDTO>();
            CreateMap<SessionLogsCreateDTO, SessionLog>();
            CreateMap<AllSessionLogsResponse, GetAllSessionLogsApiResponse>();
            CreateMap<GetAllSessionLogsApiResponse, GetAllSessionLogsApiResponse>();
            CreateMap<AllSessionLogsResponse, GetAllSessionLogsApiResponse>();
            CreateMap<GetAllSessionLogsApiResponse, AllSessionLogsResponse>();
            CreateMap<SessionLogsApiResponse, SessionLog>();
            CreateMap<SessionLog, SessionLogsApiResponse>();
        }
    }
}
