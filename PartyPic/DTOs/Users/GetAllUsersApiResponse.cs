using PartyPic.Models.Common;
using System.Collections.Generic;

namespace PartyPic.DTOs.Users
{
    public class GetAllUsersApiResponse : ApiResponse
    {
        public List<UserReadDTO> Users { get; set; }
    }
}
