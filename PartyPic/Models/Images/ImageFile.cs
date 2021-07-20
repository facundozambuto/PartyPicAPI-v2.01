using Microsoft.AspNetCore.Http;

namespace PartyPic.Models.Images
{
    public class ImageFile
    {
        public int EventId { get; set; }
        public string ProfileId { get; set; }
        public string Comment { get; set; }
        public string ProfileName { get; set; }
        public string ProfileImageUrl { get; set; }
        public IFormFile Image { get; set; }
    }
}
