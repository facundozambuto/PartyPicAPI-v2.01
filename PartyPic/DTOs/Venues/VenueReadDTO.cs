using System;

namespace PartyPic.DTOs.Venues
{
    public class VenueReadDTO
    {
        public int VenueId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public DateTime CreatedDatetime { get; set; }
        public int UserId { get; set; }
        public string Phone { get; set; }
        public string ManagerName { get; set; }
        public string ManagerEmail { get; set; }
        public string ManagerMobilePhone { get; set; }
        public string ManagerAddress { get; set; }
        public string ManagerCuil { get; set; }
    }
}
