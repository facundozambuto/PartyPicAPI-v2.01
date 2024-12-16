using PartyPic.DTOs.Events;
using PartyPic.Models.Common;
using PartyPic.Models.Events;
using System.Threading.Tasks;

namespace PartyPic.Contracts.Events
{
    public interface IEventRepository
    {
        AllEventsResponse GetAllEventsPublic();
        AllEventsResponse GetAllEvents();
        EventReadDTO GetEventById(int id);
        Task<Event> CreateEventAsync(Event ev);
        bool SaveChanges();
        void DeleteEvent(int id);
        EventReadDTO UpdateEvent(int id, EventUpdateDTO ev);
        void PartiallyUpdate(int id, EventUpdateDTO ev);
        EventGrid GetAllEventsForGrid(GridRequest gridRequest);
        void SendInstructionsByEmail(int id);
        EventReadDTO GetEventByEventCode(string eventCode);
    }
}
