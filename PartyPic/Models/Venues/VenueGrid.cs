using PartyPic.Models.Common;
using System.Collections.Generic;

namespace PartyPic.Models.Venues
{
    public class VenueGrid : ApiResponse
    {
        public List<Venue> Rows { get; set; }
        public int Total { get; set; }
        public int RowCount { get; set; }
        public int Current { get; set; }
    }
}
