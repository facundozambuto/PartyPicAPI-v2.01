using AutoMapper;
using PartyPic.DTOs.Events;
using PartyPic.Models.Events;

namespace PartyPic.Profiles.Events
{
    public class EventProfile : Profile
    {
        public EventProfile()
        {
            CreateMap<Event, EventReadDTO>();
            CreateMap<EventCreateDTO, Event>();
            CreateMap<EventGrid, EventReadDTOGrid>();
            CreateMap<EventReadDTOGrid, EventGrid>();
            CreateMap<EventUpdateDTO, Event>();
            CreateMap<Event, EventUpdateDTO>();
            CreateMap<AllEventsResponse, GetAllEventsApiResponse>();
            CreateMap<GetAllEventsApiResponse, AllEventsResponse>();
            CreateMap<AllEventsResponse, GetAllEventsApiResponse>();
            CreateMap<GetAllEventsApiResponse, AllEventsResponse>();
            CreateMap<EventGrid, GridEventApiResponse>();
            CreateMap<GridEventApiResponse, EventGrid>();
            CreateMap<EventApiResponse, Event>();
            CreateMap<Event, EventApiResponse>();
        }
    }
}
