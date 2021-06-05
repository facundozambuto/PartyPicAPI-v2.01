using PartyPic.DTOs.Events;
using PartyPic.Models.Common;
using PartyPic.Models.Events;

namespace PartyPic.Contracts.Events
{
    public interface IEventRepository
    {
        AllEventsResponse GetAllEvents();
        Event GetEventById(int id);
        Event CreateEvent(Event ev);
        bool SaveChanges();
        void DeleteEvent(int id);
        Event UpdateEvent(int id, EventUpdateDTO ev);
        void PartiallyUpdate(int id, EventUpdateDTO ev);
        EventGrid GetAllEventsForGrid(GridRequest gridRequest);
    }
}
