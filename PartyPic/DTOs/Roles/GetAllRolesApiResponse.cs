using PartyPic.Models.Common;
using System.Collections.Generic;

namespace PartyPic.DTOs.Roles
{
    public class GetAllRolesApiResponse : ApiResponse
    {
        public List<RoleReadDTO> Roles { get; set; }
    }
}
