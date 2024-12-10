using PartyPic.Models.Images;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PartyPic.Contracts.Images
{
    public interface IImagesRepository
    {
        Task<IEnumerable<Image>> GetAllEventImagesAsync(int eventId, bool firstRequest, string requestTime);
        Image GetImageById(int imageId);
        Image AddEventImage(Image image, string fileName);
        bool SaveChanges();
        IEnumerable<Image> GetAllRemovedEventImages(int eventId, string requestTime);
        Task UploadImage(ImageFile uploadImage);
        Task DeleteImageAsync(DeleteImageRequest deleteImageRequest);
        Task<byte[]> DownloadImagesAsZipAsync(int eventId);
    }
}
