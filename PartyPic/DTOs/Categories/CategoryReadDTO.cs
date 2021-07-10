using System;

namespace PartyPic.DTOs.Categories
{
    public class CategoryReadDTO
    {
        public int CategoryId { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDatetime { get; set; }
    }
}
