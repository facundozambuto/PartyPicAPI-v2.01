using PartyPic.DTOs.Events;
using System.Collections.Generic;

namespace PartyPic.Models.Events
{
    public class AllEventsResponse
    {
        public List<EventReadDTO> Events { get; set; }
    }
}
