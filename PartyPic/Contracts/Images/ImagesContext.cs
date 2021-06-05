using Microsoft.EntityFrameworkCore;
using PartyPic.Models.Images;

namespace PartyPic.Contracts.Images
{
    public class ImagesContext : DbContext
    {
        public ImagesContext(DbContextOptions<ImagesContext> options) : base(options)
        {

        }

        public DbSet<Image> Images { get; set; }
    }
}
