using PartyPic.Models.Common;
using System.Collections.Generic;

namespace PartyPic.DTOs.Venues
{
    public class GetAllVenuesApiResponse : ApiResponse
    {
        public List<VenueReadDTO> Venues { get; set; }
    }
}
