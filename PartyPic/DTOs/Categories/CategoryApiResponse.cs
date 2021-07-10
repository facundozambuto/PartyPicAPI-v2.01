using PartyPic.Models.Common;

namespace PartyPic.DTOs.Categories
{
    public class CategoryApiResponse : ApiResponse
    {
        public int CategoryId { get; set; }
        public string Description { get; set; }
    }
}
