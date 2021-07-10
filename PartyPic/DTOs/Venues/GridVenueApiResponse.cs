using PartyPic.Models.Common;
using System.Collections.Generic;

namespace PartyPic.DTOs.Venues
{
    public class GridVenueApiResponse : ApiResponse
    {
        public List<VenueReadDTO> Rows { get; set; }
        public int Total { get; set; }
        public int RowCount { get; set; }
        public int Current { get; set; }
    }
}
