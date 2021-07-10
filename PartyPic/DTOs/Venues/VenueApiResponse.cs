using PartyPic.Models.Common;
using System;

namespace PartyPic.DTOs.Venues
{
    public class VenueApiResponse : ApiResponse
    {
        public int VenueId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public DateTime CreatedDatetime { get; set; }
        public int UserId { get; set; }
        public string Phone { get; set; }
    }
}
