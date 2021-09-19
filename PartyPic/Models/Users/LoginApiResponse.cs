using PartyPic.Models.Common;

namespace PartyPic.Models.Users
{
    public class LoginApiResponse : ApiResponse
    {
        public string Email { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
    }
}
