using PartyPic.Models.Common;
using System.Collections.Generic;

namespace PartyPic.DTOs.Categories
{
    public class GetAllCategoriesApiResponse : ApiResponse
    {
        public List<CategoryReadDTO> Categories { get; set; }
    }
}
