using PartyPic.Models.Common;
using System.Collections.Generic;

namespace PartyPic.DTOs.Categories
{
    public class GridCategoryApiResponse : ApiResponse
    {
        public List<CategoryReadDTO> Rows { get; set; }
        public int Total { get; set; }
        public int RowCount { get; set; }
        public int Current { get; set; }
    }
}
