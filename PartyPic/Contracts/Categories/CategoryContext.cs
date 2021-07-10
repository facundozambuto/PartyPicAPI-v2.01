using Microsoft.EntityFrameworkCore;
using PartyPic.Models.Categories;

namespace PartyPic.Contracts.Categories
{
    public class CategoryContext : DbContext
    {
        public CategoryContext(DbContextOptions<CategoryContext> options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }
    }
}
