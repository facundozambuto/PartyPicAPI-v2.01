using System.Collections.Generic;

namespace PartyPic.DTOs.Venues
{
    public class VenueReadDTOGrid
    {
        public List<VenueReadDTO> Rows { get; set; }
        public int Total { get; set; }
        public int RowCount { get; set; }
        public int Current { get; set; }
    }
}
