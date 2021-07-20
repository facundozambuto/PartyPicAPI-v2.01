using PartyPic.DTOs.Events;
using PartyPic.Models.Common;
using System.Collections.Generic;

namespace PartyPic.Models.Events
{
    public class EventGrid : ApiResponse
    {
        public List<EventReadDTO> Rows { get; set; }
        public int Total { get; set; }
        public int RowCount { get; set; }
        public int Current { get; set; }
    }
}
