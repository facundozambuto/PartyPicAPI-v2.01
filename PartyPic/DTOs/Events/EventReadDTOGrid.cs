using System.Collections.Generic;

namespace PartyPic.DTOs.Events
{
    public class EventReadDTOGrid
    {
        public List<EventReadDTO> Rows { get; set; }
        public int Total { get; set; }
        public int RowCount { get; set; }
        public int Current { get; set; }
    }
}
