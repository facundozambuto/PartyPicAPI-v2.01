namespace PartyPic.Models.Images
{
    public class DeleteImageRequest
    {
        public int ImageId { get; set; }
        public int EventId { get; set; }
        public int UserId { get; set; }
        public string ProfileId { get; set; }
        public string ProfileName { get; set; }
        public bool BlockProfile { get; set; }
    }
}
