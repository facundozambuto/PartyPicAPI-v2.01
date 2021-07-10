using PartyPic.Models.Common;

namespace PartyPic.DTOs.Roles
{
    public class RoleApiResponse : ApiResponse
    {
        public int RoleId { get; set; }
        public string Description { get; set; }
    }
}
