using PartyPic.Models.Common;
using System.Collections.Generic;

namespace PartyPic.DTOs.Events
{
    public class GetAllEventsApiResponse : ApiResponse
    {
        public List<EventReadDTO> Events { get; set; }
    }
}
