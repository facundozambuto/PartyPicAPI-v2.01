using System;

namespace PartyPic.DTOs.Images
{
    public class ImageReadDTO
    {
        public int ImageId { get; set; }
        public string Path { get; set; }
        public int EventId { get; set; }
        public string ProfileId { get; set; }
        public DateTime CreatedDatetime { get; set; }
        public string Comment { get; set; }
        public string ProfileName { get; set; }
        public string ProfileImageUrl { get; set; }
        public DateTime? DeletedDatetime { get; set; }
    }
}
