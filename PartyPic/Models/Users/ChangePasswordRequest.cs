namespace PartyPic.Models.Users
{
    public class ChangePasswordRequest
    {
        public int UserId { get; set; }
        public string Password { get; set; }
    }
}
