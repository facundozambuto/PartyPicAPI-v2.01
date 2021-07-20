using PartyPic.Models.Common;
using System;

namespace PartyPic.DTOs.Events
{
    public class EventApiResponse : ApiResponse
    {
        public int EventId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int VenueId { get; set; }
        public DateTime StartDatetime { get; set; }
        public string QRCode { get; set; }
        public bool Enabled { get; set; }
        public DateTime LastRequest { get; set; }
        public DateTime CreatedDatetime { get; set; }
        public int CategoryId { get; set; }
        public string CategoryDescription { get; set; }
        public string VenueName { get; set; }
        public string Name { get; set; }
    }
}
