using PartyPic.DTOs.Events;
using PartyPic.Models.Common;
using PartyPic.Models.Events;

namespace PartyPic.Contracts.Events
{
    public interface IEventRepository
    {
        AllEventsResponse GetAllEvents();
        EventReadDTO GetEventById(int id);
        Event CreateEvent(Event ev);
        bool SaveChanges();
        void DeleteEvent(int id);
        EventReadDTO UpdateEvent(int id, EventUpdateDTO ev);
        void PartiallyUpdate(int id, EventUpdateDTO ev);
        EventGrid GetAllEventsForGrid(GridRequest gridRequest);
        void SendInstructionsByEmail(int id);
        EventReadDTO GetEventByEventCode(string eventCode);
    }
}
